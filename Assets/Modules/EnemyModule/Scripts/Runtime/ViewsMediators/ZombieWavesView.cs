using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using FlowIoC.PoolModule.Services;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Entities;
using Modules.EnemyModule.Enums;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EnemyModule.ViewsMediators
{
    /// <summary>
    /// The match scene's zombie wave settings and spawn points, filed under EnemySystemRoot. Every
    /// child of the spawn point root is a point zombies come in at - move them, add or remove them in
    /// the scene. The wave and its countdown are replicated from here to every client.
    /// </summary>
    public struct NetworkTurret : INetworkSerializable, IEquatable<NetworkTurret>
    {
        public int Id;
        public int Kind;
        public int Level;
        public Vector3 Position;
        public float Yaw;
        public int Health;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Kind);
            serializer.SerializeValue(ref Level);
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Yaw);
            serializer.SerializeValue(ref Health);
        }

        public bool Equals(NetworkTurret other) => Id == other.Id && Level == other.Level && Yaw.Equals(other.Yaw) && Health == other.Health;
    }

    [RequireComponent(typeof(ViewInjector))]
    public class ZombieWavesView : NetworkBehaviour, IView, IZombieWaveBoard
    {
        [SerializeField] private CD_ZombieWaves _config;
        [SerializeField] private Transform _spawnPointRoot;
        [Tooltip("The height of the walkable ground in this scene. Players and zombies are centred a metre above it, so what lies on the ground - structures, corpses, rings - is drawn here rather than at their centre height, where it would hide their legs.")]
        [SerializeField] private float _groundHeight = -1f;
        [SerializeField] private CD_Turret _turret;
        [SerializeField] private CD_Barricade _barricade;
        [SerializeField] private CD_Wall _wall;
        [SerializeField] private CD_Barracks _barracks;
        [SerializeField] private CD_Repair _repair;
        [SerializeField] private CD_DayNight _dayNight;
        [Tooltip("The sun the day and night turn; left empty, the light stays as the scene has it.")]
        [SerializeField] private Light _sun;
        [SerializeField] private CD_Gate _gate;
        [SerializeField] private CD_SpikeTrap _spikeTrap;
        [SerializeField] private CD_Mine _mine;
        [SerializeField] private CD_RepairStation _repairStation;
        [Tooltip("Left empty on a map with no headquarters to defend.")]
        [SerializeField] private CD_Headquarters _headquarters;
        [Tooltip("Every child is a wall the base starts with, facing the child's forward. With a layout, the root is only the base's centre.")]
        [SerializeField] private Transform _baseWallRoot;
        [Tooltip("The base's shape; left empty, the base walls are the root's children.")]
        [SerializeField] private CD_BaseLayout _baseLayout;
        [SerializeField] private CD_Objective _objective;
        [Tooltip("Every child is a point the defend objective may appear at.")]
        [SerializeField] private Transform _objectivePointRoot;

        [Tooltip("The pool key the turrets are drawn from; the group is filed on EnemySystemRoot.")]
        [SerializeField] private string _turretPoolKey = "Turret";
        [SerializeField] private string _barricadePoolKey = "Barricade";
        [SerializeField] private string _wallPoolKey = "Wall";
        [SerializeField] private string _barracksPoolKey = "Barracks";
        [SerializeField] private string _gatePoolKey = "Gate";
        [SerializeField] private string _spikeTrapPoolKey = "SpikeTrap";
        [SerializeField] private string _minePoolKey = "Mine";
        [SerializeField] private string _repairStationPoolKey = "RepairStation";
        [SerializeField] private string _headquartersPoolKey = "Headquarters";
        [SerializeField] private string _damageNumberPoolKey = "DamageNumber";
        [SerializeField] private string _corpsePoolKey = "ZombieCorpse";
        [SerializeField] private string _blastPoolKey = "Blast";
        [SerializeField] private string _spitPoolKey = "Spit";
        [SerializeField] private string _objectivePoolKey = "ObjectiveZone";
        [Tooltip("A hit at least this strong shows as a heavy hit.")]
        [SerializeField] private int _heavyHitDamage = 25;

        private NetworkList<NetworkTurret> _turrets;
        private readonly Dictionary<int, IStructureVisual> _turretVisuals = new();
        private IPoolService _pool;

        private readonly NetworkVariable<int> _wave = new(0,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        // Server time the next wave comes in at; zero while a wave is on.
        private readonly NetworkVariable<double> _nextWaveTime = new(0d,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<int> _zombiesLeft = new(0,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public bool IsRegistered { get; set; }

        /// <summary>The wave, the seconds until it starts (zero once it is on), and the zombies left.</summary>
        public Action<int, float, int> StateChanged;
        public Action<int> StructureGranted;
        public Action<ulong, Vector3, int> StructurePlaceRequested;
        public Action<int> WaveCleared;
        public Action<Vector3> ZombieHitShown;
        public Action<Vector3> ZombieDeathShown;
        public Action<Vector3> BlastShown;
        public Action<Vector3> StructureFellShown;

        // Chunks thrown by hits, deaths, blasts and falls; one particle system serves them all.
        private ChunkBurst _chunks;

        /// <summary>The boss's name, health and most health; an empty name when there is none.</summary>
        public Action<string, int, int> BossChanged;

        /// <summary>The objective's title, whether it is on, and how far it is held, 0 to 1.</summary>
        public Action<string, bool, float> ObjectiveChanged;

        /// <summary>The objective ended: held or not, and what it paid.</summary>
        public Action<bool, int> ObjectiveEnded;

        private readonly NetworkVariable<bool> _objectiveActive = new();
        private readonly NetworkVariable<Vector3> _objectivePosition = new();
        private readonly NetworkVariable<float> _objectiveProgress = new();
        private ObjectiveZone _objectiveZone;

        private readonly NetworkVariable<Unity.Collections.FixedString64Bytes> _bossName = new();
        private readonly NetworkVariable<int> _bossHealth = new();
        private readonly NetworkVariable<int> _bossMaxHealth = new();

        private const float CARRY_HEIGHT = 2.3f;
        private const float CARRY_SCALE = 0.6f;

        private IStructureVisual _carried;
        private int _carriedKind = -1;

        public CD_ZombieWaves Config => _config;
        public CD_Turret TurretConfig => _turret;
        public CD_Barricade BarricadeConfig => _barricade;
        public CD_Wall WallConfig => _wall;
        public CD_Barracks BarracksConfig => _barracks;
        public CD_Repair RepairConfig => _repair;
        public CD_DayNight DayNightConfig => _dayNight;

        private readonly NetworkVariable<bool> _bloodMoon = new();

        public void SetBloodMoonServer(bool isBloodMoon)
        {
            if (IsServer)
                _bloodMoon.Value = isBloodMoon;
        }

        // Night while a wave is on, day in the pause before the next; the light eases between them.
        private void TurnLight()
        {
            if (_sun == null || _dayNight == null || !IsSpawned)
                return;

            bool isNight = _wave.Value > 0 && _nextWaveTime.Value <= 0d;
            Color color = !isNight ? _dayNight.DayColor : _bloodMoon.Value ? _dayNight.BloodMoonColor : _dayNight.NightColor;
            float intensity = !isNight ? _dayNight.DayIntensity : _bloodMoon.Value ? _dayNight.BloodMoonIntensity : _dayNight.NightIntensity;
            float ease = Time.deltaTime / Mathf.Max(0.01f, _dayNight.Transition);
            _sun.color = Color.Lerp(_sun.color, color, ease * 3f);
            _sun.intensity = Mathf.Lerp(_sun.intensity, intensity, ease * 3f);
        }
        public CD_Gate GateConfig => _gate;
        public CD_SpikeTrap SpikeTrapConfig => _spikeTrap;
        public CD_Mine MineConfig => _mine;
        public CD_RepairStation RepairStationConfig => _repairStation;
        public CD_Headquarters HeadquartersConfig => _headquarters;
        public CD_BaseLayout BaseLayout => _baseWallRoot != null ? _baseLayout : null;
        public Vector3 BaseCenter => _baseWallRoot != null ? _baseWallRoot.position : transform.position;

        /// <summary>A player asked to mend what they stand by: their client id.</summary>
        public Action<ulong> RepairRequested;

        public void RequestRepair() => RepairServerRpc();

        /// <summary>A player asked to switch their soldiers' order: their client id.</summary>
        public Action<ulong> AllyOrderRequested;

        /// <summary>The local player's soldiers now follow them, or guard.</summary>
        public Action<bool> AllyOrderAnnounced;

        public void RequestAllyOrder() => AllyOrderServerRpc();

        /// <summary>A player asked for the next wave now.</summary>
        public Action SkipBreakRequested;

        public void RequestSkipBreak() => SkipBreakServerRpc();

        [ServerRpc(RequireOwnership = false)]
        private void SkipBreakServerRpc() => SkipBreakRequested?.Invoke();

        public void AnnounceAllyOrderServer(ulong clientId, bool isFollowing)
        {
            if (!IsServer)
                return;

            var target = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } } };
            AllyOrderClientRpc(isFollowing, target);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AllyOrderServerRpc(ServerRpcParams rpcParams = default) => AllyOrderRequested?.Invoke(rpcParams.Receive.SenderClientId);

        [ClientRpc]
        private void AllyOrderClientRpc(bool isFollowing, ClientRpcParams rpcParams = default) => AllyOrderAnnounced?.Invoke(isFollowing);

        [ServerRpc(RequireOwnership = false)]
        private void RepairServerRpc(ServerRpcParams rpcParams = default) => RepairRequested?.Invoke(rpcParams.Receive.SenderClientId);
        public int BaseWallCount => _baseWallRoot != null ? _baseWallRoot.childCount : 0;
        public Vector3 GetBaseWallPoint(int index) => _baseWallRoot.GetChild(index).position;
        public float GetBaseWallYaw(int index) => _baseWallRoot.GetChild(index).eulerAngles.y;
        public CD_Objective ObjectiveConfig => _objective;
        public int ObjectivePointCount => _objectivePointRoot != null ? _objectivePointRoot.childCount : 0;
        public Vector3 GetObjectivePoint(int index) => _objectivePointRoot.GetChild(index).position;
        public int SpawnPointCount => _spawnPointRoot != null ? _spawnPointRoot.childCount : 0;
        public Vector3 GetSpawnPoint(int index) => _spawnPointRoot.GetChild(index).position;

        private void Awake() => _turrets = new NetworkList<NetworkTurret>();

        /// <summary>The pool the turrets are drawn from, handed over by the Mediator.</summary>
        public void Configure(IPoolService pool)
        {
            _pool = pool;
            if (!IsSpawned)
                return;

            DrawTurrets();
            DrawObjective();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _turrets.OnListChanged += OnTurretsChanged;
            DrawTurrets();
            _wave.OnValueChanged += OnIntChanged;
            _zombiesLeft.OnValueChanged += OnIntChanged;
            _nextWaveTime.OnValueChanged += OnTimeChanged;
            _bossName.OnValueChanged += OnBossNameChanged;
            _bossHealth.OnValueChanged += OnBossValueChanged;
            _bossMaxHealth.OnValueChanged += OnBossValueChanged;
            _objectiveActive.OnValueChanged += OnObjectiveActiveChanged;
            _objectiveProgress.OnValueChanged += OnObjectiveProgressChanged;
            DrawObjective();
            Publish();
            PublishBoss();
        }

        public override void OnNetworkDespawn()
        {
            _turrets.OnListChanged -= OnTurretsChanged;
            _wave.OnValueChanged -= OnIntChanged;
            _zombiesLeft.OnValueChanged -= OnIntChanged;
            _nextWaveTime.OnValueChanged -= OnTimeChanged;
            _bossName.OnValueChanged -= OnBossNameChanged;
            _bossHealth.OnValueChanged -= OnBossValueChanged;
            _bossMaxHealth.OnValueChanged -= OnBossValueChanged;
            _objectiveActive.OnValueChanged -= OnObjectiveActiveChanged;
            _objectiveProgress.OnValueChanged -= OnObjectiveProgressChanged;
            _objectiveZone?.Dismiss();
            _objectiveZone = null;

            foreach (IStructureVisual visual in _turretVisuals.Values)
                visual.Dismiss();
            _turretVisuals.Clear();
            ShowCarried(-1);

            base.OnNetworkDespawn();
        }

        public void SetCountdownServer(int nextWave, float seconds)
        {
            if (!IsServer)
                return;

            _wave.Value = nextWave;
            _zombiesLeft.Value = 0;
            _nextWaveTime.Value = NetworkManager.ServerTime.Time + seconds;
        }

        public void SetWaveServer(int wave, int zombiesLeft)
        {
            if (!IsServer)
                return;

            _wave.Value = wave;
            _zombiesLeft.Value = zombiesLeft;
            _nextWaveTime.Value = 0d;
        }

        public void SetStructureServer(int id, int kind, int level, Vector3 position, float yaw, int health)
        {
            if (!IsServer)
                return;

            var turret = new NetworkTurret { Id = id, Kind = kind, Level = level, Position = position, Yaw = yaw, Health = health };
            for (int i = 0; i < _turrets.Count; i++)
            {
                if (_turrets[i].Id != id)
                    continue;

                if (!_turrets[i].Equals(turret))
                    _turrets[i] = turret;
                return;
            }

            _turrets.Add(turret);
        }

        public void RemoveStructureServer(int id)
        {
            if (!IsServer)
                return;

            for (int i = 0; i < _turrets.Count; i++)
            {
                if (_turrets[i].Id != id)
                    continue;

                _turrets.RemoveAt(i);
                return;
            }
        }

        public void ShowTurretShotServer(int id, Vector3 target)
        {
            if (IsServer)
                TurretShotClientRpc(id, target);
        }

        public void GrantStructureServer(ulong clientId, int kind)
        {
            if (!IsServer)
                return;

            var target = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } } };
            StructureGrantedClientRpc(kind, target);
        }

        public void RequestPlaceStructure(Vector3 position, int kind) => PlaceStructureServerRpc(position, kind);

        public void ShowCarried(int kind)
        {
            if (kind == _carriedKind)
                return;

            _carried?.Dismiss();
            _carried = null;
            _carriedKind = kind;

            if (kind < 0 || _pool == null)
                return;

            _carried = Get(kind);
            _carried?.Place(transform.position, CARRY_SCALE, false);
        }

        private Vector3 Grounded(Vector3 position) => new(position.x, _groundHeight, position.z);

        private IStructureVisual Get(int kind) => (StructureKind) kind switch
        {
            StructureKind.Barricade => _pool.Get<BarricadeVisual>(_barricadePoolKey, transform),
            StructureKind.Wall => _pool.Get<BarricadeVisual>(_wallPoolKey, transform),
            StructureKind.Barracks => _pool.Get<BarricadeVisual>(_barracksPoolKey, transform),
            StructureKind.Gate => _pool.Get<BarricadeVisual>(_gatePoolKey, transform),
            StructureKind.SpikeTrap => _pool.Get<BarricadeVisual>(_spikeTrapPoolKey, transform),
            StructureKind.Mine => _pool.Get<BarricadeVisual>(_minePoolKey, transform),
            StructureKind.RepairStation => _pool.Get<BarricadeVisual>(_repairStationPoolKey, transform),
            StructureKind.Headquarters => _pool.Get<BarricadeVisual>(_headquartersPoolKey, transform),
            _ => _pool.Get<TurretVisual>(_turretPoolKey, transform)
        };

        // A turret wears its level's colour and a wall its tier's; a clear colour leaves the prefab's own.
        private Color Tint(NetworkTurret structure) => (StructureKind) structure.Kind switch
        {
            StructureKind.Turret => _turret != null ? _turret.LevelColor(structure.Level) : Color.white,
            StructureKind.Wall => _wall != null ? _wall.Tier(structure.Level).Color : Color.clear,
            _ => Color.clear
        };

        private int MaxHealth(NetworkTurret structure) => (StructureKind) structure.Kind switch
        {
            StructureKind.Barricade => _barricade != null ? _barricade.Health : 1,
            StructureKind.Wall => _wall != null ? _wall.Tier(structure.Level).Health : 1,
            StructureKind.Barracks => _barracks != null ? _barracks.Health : 1,
            StructureKind.SpikeTrap => _spikeTrap != null ? _spikeTrap.Health : 1,
            StructureKind.Mine => _mine != null ? _mine.Health : 1,
            StructureKind.Gate => _gate != null ? _gate.Health : 1,
            StructureKind.RepairStation => _repairStation != null ? _repairStation.Health : 1,
            StructureKind.Headquarters => _headquarters != null ? _headquarters.Health : 1,
            _ => _turret != null ? _turret.Level(structure.Level).Health : 1
        };

        public void ShowZombieHitServer(Vector3 position, int damage)
        {
            if (IsServer)
                ZombieHitClientRpc(position, damage);
        }

        public void ShowZombieDeathServer(Vector3 position, float yaw, Color tint, float scale)
        {
            if (IsServer)
                ZombieDeathClientRpc(position, yaw, tint, scale);
        }

        public void ShowBlastServer(Vector3 position, float radius, Color color)
        {
            if (IsServer)
                BlastClientRpc(position, radius, color);
        }

        public void ShowSpitServer(Vector3 position, Vector3 velocity, float lifetime)
        {
            if (IsServer)
                SpitClientRpc(position, velocity, lifetime);
        }

        public void SetBossServer(string name, int health, int maxHealth)
        {
            if (!IsServer)
                return;

            _bossName.Value = name;
            _bossMaxHealth.Value = maxHealth;
            _bossHealth.Value = health;
        }

        [ClientRpc]
        private void BlastClientRpc(Vector3 position, float radius, Color color)
        {
            if (_pool != null)
                _pool.Get<BlastRing>(_blastPoolKey, transform)?.Show(Grounded(position), radius, color);

            Chunks().Burst(position, new Color(0.25f, 0.2f, 0.15f), 14 + Mathf.RoundToInt(radius * 3f), 7f, 0.22f);
            Chunks().Burst(position, color, 8, 9f, 0.16f);
            BlastShown?.Invoke(position);
        }

        private ChunkBurst Chunks()
        {
            if (_chunks == null)
                _chunks = ChunkBurst.Create(transform);

            return _chunks;
        }

        [ClientRpc]
        private void SpitClientRpc(Vector3 position, Vector3 velocity, float lifetime)
        {
            if (_pool != null)
                _pool.Get<SpitProjectile>(_spitPoolKey, transform)?.Launch(position, velocity, lifetime);
        }

        private void OnBossNameChanged(Unity.Collections.FixedString64Bytes previous, Unity.Collections.FixedString64Bytes current) => PublishBoss();

        private void OnBossValueChanged(int previous, int current) => PublishBoss();

        private void PublishBoss() => BossChanged?.Invoke(_bossName.Value.ToString(), _bossHealth.Value, _bossMaxHealth.Value);

        public void SetObjectiveServer(bool isActive, Vector3 position, float progress)
        {
            if (!IsServer)
                return;

            _objectivePosition.Value = position;
            _objectiveProgress.Value = progress;
            _objectiveActive.Value = isActive;
        }

        public void AnnounceObjectiveServer(bool isHeld, int reward)
        {
            if (IsServer)
                ObjectiveEndedClientRpc(isHeld, reward);
        }

        [ClientRpc]
        private void ObjectiveEndedClientRpc(bool isHeld, int reward) => ObjectiveEnded?.Invoke(isHeld, reward);

        private void OnObjectiveActiveChanged(bool previous, bool current) => DrawObjective();

        private void OnObjectiveProgressChanged(float previous, float current) => DrawObjective();

        // The point shows while the objective is on; the HUD hears its title and progress.
        private void DrawObjective()
        {
            bool isActive = _objectiveActive.Value && _objective != null;
            if (isActive && _objectiveZone == null && _pool != null)
                _objectiveZone = _pool.Get<ObjectiveZone>(_objectivePoolKey, transform);

            if (!isActive && _objectiveZone != null)
            {
                _objectiveZone.Dismiss();
                _objectiveZone = null;
            }

            _objectiveZone?.Show(Grounded(_objectivePosition.Value), _objective.Radius, _objectiveProgress.Value);
            ObjectiveChanged?.Invoke(_objective != null ? _objective.Title : string.Empty, isActive, _objectiveProgress.Value);
        }

        public void AnnounceWaveClearedServer(int wave)
        {
            if (IsServer)
                WaveClearedClientRpc(wave);
        }

        [ClientRpc]
        private void ZombieHitClientRpc(Vector3 position, int damage)
        {
            if (_pool != null)
                _pool.Get<DamageNumber>(_damageNumberPoolKey, transform)?.Show(position, damage, damage >= _heavyHitDamage);

            Chunks().Burst(position, new Color(0.55f, 0.05f, 0.05f), damage >= _heavyHitDamage ? 6 : 3, 3.5f, 0.12f);
            ZombieHitShown?.Invoke(position);
        }

        [ClientRpc]
        private void ZombieDeathClientRpc(Vector3 position, float yaw, Color tint, float scale)
        {
            if (_pool != null)
                _pool.Get<ZombieCorpse>(_corpsePoolKey, transform)?.Show(Grounded(position), yaw, tint, scale);

            Chunks().Burst(position, new Color(0.5f, 0.04f, 0.04f), 10, 4.5f, 0.16f * scale);
            Chunks().Burst(position, tint * 0.8f, 5, 3.5f, 0.2f * scale);
            ZombieDeathShown?.Invoke(position);
        }

        [ClientRpc]
        private void WaveClearedClientRpc(int wave) => WaveCleared?.Invoke(wave);

        [ClientRpc]
        private void StructureGrantedClientRpc(int kind, ClientRpcParams rpcParams = default) => StructureGranted?.Invoke(kind);

        [ServerRpc(RequireOwnership = false)]
        private void PlaceStructureServerRpc(Vector3 position, int kind, ServerRpcParams rpcParams = default) =>
            StructurePlaceRequested?.Invoke(rpcParams.Receive.SenderClientId, position, kind);

        [ClientRpc]
        private void TurretShotClientRpc(int id, Vector3 target)
        {
            if (!_turretVisuals.TryGetValue(id, out IStructureVisual visual))
                return;

            visual.ShowShot(target);
        }

        private void Update()
        {
            TurnLight();

            // The carried turret rides over the local player's head wherever they walk.
            if (_carried == null)
                return;

            NetworkObject player = NetworkManager != null ? NetworkManager.LocalClient?.PlayerObject : null;
            if (player != null)
                _carried.transform.position = player.transform.position + Vector3.up * CARRY_HEIGHT;
        }

        private void OnTurretsChanged(NetworkListEvent<NetworkTurret> change) => DrawTurrets();

        // The turrets each client draws, each one taken from the pool and returned when it is gone.
        private void DrawTurrets()
        {
            if (_pool == null)
                return;

            var present = new HashSet<int>();
            foreach (NetworkTurret turret in _turrets)
            {
                present.Add(turret.Id);
                if (!_turretVisuals.TryGetValue(turret.Id, out IStructureVisual visual))
                {
                    visual = Get(turret.Kind);
                    if (visual == null)
                        continue;

                    visual.Place(Grounded(turret.Position), 1f, true);
                    _turretVisuals[turret.Id] = visual;
                }

                int maxHealth = MaxHealth(turret);
                Color color = Tint(turret);
                visual.SetState(turret.Yaw, Mathf.Clamp01((float) turret.Health / Mathf.Max(1, maxHealth)), color);
            }

            var gone = new List<int>();
            foreach (int id in _turretVisuals.Keys)
            {
                if (!present.Contains(id))
                    gone.Add(id);
            }

            foreach (int id in gone)
            {
                // Taken off the board mid-match means it fell: it breaks apart where it stood.
                Vector3 position = _turretVisuals[id].transform.position;
                Chunks().Burst(position, new Color(0.45f, 0.38f, 0.3f), 22, 6f, 0.28f);
                StructureFellShown?.Invoke(position);

                _turretVisuals[id].Dismiss();
                _turretVisuals.Remove(id);
            }
        }

        private void OnIntChanged(int previous, int current) => Publish();

        private void OnTimeChanged(double previous, double current) => Publish();

        private void Publish()
        {
            float seconds = _nextWaveTime.Value > 0d
                ? Mathf.Max(0f, (float) (_nextWaveTime.Value - NetworkManager.ServerTime.Time))
                : 0f;

            StateChanged?.Invoke(_wave.Value, seconds, _zombiesLeft.Value);
        }

        private void OnDrawGizmos()
        {
            if (_spawnPointRoot == null)
                return;

            Gizmos.color = new Color(0.4f, 0.9f, 0.3f, 0.8f);
            foreach (Transform point in _spawnPointRoot)
                Gizmos.DrawWireSphere(point.position, 0.75f);

            if (_baseWallRoot == null)
                return;

            Gizmos.color = new Color(0.9f, 0.7f, 0.3f, 0.8f);
            if (_baseLayout != null)
            {
                float side = _baseLayout.HalfSize * 2f;
                Gizmos.DrawWireCube(_baseWallRoot.position + Vector3.up, new Vector3(side, 2f, side));
                return;
            }

            foreach (Transform point in _baseWallRoot)
                Gizmos.DrawWireCube(point.position + Vector3.up, new Vector3(2f, 2f, 2f));
        }
    }
}
