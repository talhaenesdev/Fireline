using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Entities;
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
        public Vector3 Position;
        public float Yaw;
        public int Health;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Yaw);
            serializer.SerializeValue(ref Health);
        }

        public bool Equals(NetworkTurret other) => Id == other.Id && Yaw.Equals(other.Yaw) && Health == other.Health;
    }

    [RequireComponent(typeof(ViewInjector))]
    public class ZombieWavesView : NetworkBehaviour, IView, IZombieWaveBoard
    {
        [SerializeField] private CD_ZombieWaves _config;
        [SerializeField] private Transform _spawnPointRoot;
        [SerializeField] private CD_Turret _turret;

        private NetworkList<NetworkTurret> _turrets;
        private readonly Dictionary<int, TurretVisual> _turretVisuals = new();

        private class TurretVisual
        {
            public Transform Root;
            public Transform Head;
            public Transform HealthFill;
            public LineRenderer Tracer;
            public float TracerUntil;
        }

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
        public Action TurretGranted;
        public Action<ulong, Vector3> TurretPlaceRequested;

        private Transform _preview;
        private Renderer[] _previewRenderers;

        public CD_ZombieWaves Config => _config;
        public CD_Turret TurretConfig => _turret;
        public int SpawnPointCount => _spawnPointRoot != null ? _spawnPointRoot.childCount : 0;
        public Vector3 GetSpawnPoint(int index) => _spawnPointRoot.GetChild(index).position;

        private void Awake() => _turrets = new NetworkList<NetworkTurret>();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _turrets.OnListChanged += OnTurretsChanged;
            DrawTurrets();
            _wave.OnValueChanged += OnIntChanged;
            _zombiesLeft.OnValueChanged += OnIntChanged;
            _nextWaveTime.OnValueChanged += OnTimeChanged;
            Publish();
        }

        public override void OnNetworkDespawn()
        {
            _turrets.OnListChanged -= OnTurretsChanged;
            _wave.OnValueChanged -= OnIntChanged;
            _zombiesLeft.OnValueChanged -= OnIntChanged;
            _nextWaveTime.OnValueChanged -= OnTimeChanged;
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

        public void SetTurretServer(int id, Vector3 position, float yaw, int health)
        {
            if (!IsServer)
                return;

            var turret = new NetworkTurret { Id = id, Position = position, Yaw = yaw, Health = health };
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

        public void RemoveTurretServer(int id)
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

        public void GrantTurretServer(ulong clientId)
        {
            if (!IsServer)
                return;

            var target = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } } };
            TurretGrantedClientRpc(target);
        }

        public void RequestPlaceTurret(Vector3 position) => PlaceTurretServerRpc(position);

        public void ShowPlacementPreview(bool isShown, Vector3 position, bool isValid)
        {
            if (!isShown)
            {
                if (_preview != null)
                    _preview.gameObject.SetActive(false);
                return;
            }

            if (_preview == null)
            {
                _preview = CreateTurret(new NetworkTurret { Id = -1, Position = position }).Root;
                _preview.name = "TurretPreview";
                _previewRenderers = _preview.GetComponentsInChildren<Renderer>(true);
                foreach (LineRenderer line in _preview.GetComponentsInChildren<LineRenderer>(true))
                    line.enabled = false;
            }

            _preview.gameObject.SetActive(true);
            _preview.position = position;

            Color tint = isValid ? new Color(0.3f, 1f, 0.4f, 1f) : new Color(1f, 0.3f, 0.25f, 1f);
            foreach (Renderer part in _previewRenderers)
            {
                if (part is not LineRenderer)
                    part.material.color = tint;
            }
        }

        [ClientRpc]
        private void TurretGrantedClientRpc(ClientRpcParams rpcParams = default) => TurretGranted?.Invoke();

        [ServerRpc(RequireOwnership = false)]
        private void PlaceTurretServerRpc(Vector3 position, ServerRpcParams rpcParams = default) =>
            TurretPlaceRequested?.Invoke(rpcParams.Receive.SenderClientId, position);

        [ClientRpc]
        private void TurretShotClientRpc(int id, Vector3 target)
        {
            if (!_turretVisuals.TryGetValue(id, out TurretVisual visual))
                return;

            visual.Tracer.SetPosition(0, visual.Head.position + visual.Head.forward * 0.6f);
            visual.Tracer.SetPosition(1, target + Vector3.up);
            visual.Tracer.enabled = true;
            visual.TracerUntil = Time.time + 0.06f;
        }

        private void Update()
        {
            foreach (TurretVisual visual in _turretVisuals.Values)
            {
                if (visual.Tracer.enabled && Time.time > visual.TracerUntil)
                    visual.Tracer.enabled = false;
            }
        }

        private void OnTurretsChanged(NetworkListEvent<NetworkTurret> change) => DrawTurrets();

        // The turrets each client draws: a base, a head that turns to what it fires at, and a health bar.
        private void DrawTurrets()
        {
            var present = new HashSet<int>();
            foreach (NetworkTurret turret in _turrets)
            {
                present.Add(turret.Id);
                if (!_turretVisuals.TryGetValue(turret.Id, out TurretVisual visual))
                    _turretVisuals[turret.Id] = visual = CreateTurret(turret);

                visual.Head.rotation = Quaternion.Euler(0f, turret.Yaw, 0f);
                float share = _turret != null && _turret.Health > 0 ? Mathf.Clamp01((float) turret.Health / _turret.Health) : 1f;
                visual.HealthFill.localScale = new Vector3(share, 1f, 1f);
                visual.HealthFill.localPosition = new Vector3((share - 1f) * 0.5f, 0f, 0f);
            }

            var gone = new List<int>();
            foreach (int id in _turretVisuals.Keys)
            {
                if (!present.Contains(id))
                    gone.Add(id);
            }

            foreach (int id in gone)
            {
                Destroy(_turretVisuals[id].Root.gameObject);
                _turretVisuals.Remove(id);
            }
        }

        private TurretVisual CreateTurret(NetworkTurret turret)
        {
            var root = new GameObject($"Turret_{turret.Id}").transform;
            root.SetParent(transform, false);
            root.position = turret.Position;

            Part(PrimitiveType.Cylinder, root, new Vector3(0f, 0.35f, 0f), new Vector3(0.9f, 0.35f, 0.9f), new Color(0.25f, 0.27f, 0.3f));

            var head = new GameObject("Head").transform;
            head.SetParent(root, false);
            head.localPosition = new Vector3(0f, 0.95f, 0f);
            Part(PrimitiveType.Cube, head, Vector3.zero, new Vector3(0.6f, 0.4f, 0.6f), new Color(0.35f, 0.55f, 0.85f));
            Part(PrimitiveType.Cube, head, new Vector3(0f, 0f, 0.45f), new Vector3(0.15f, 0.15f, 0.6f), new Color(0.15f, 0.15f, 0.18f));

            var bar = new GameObject("HealthBar").transform;
            bar.SetParent(root, false);
            bar.localPosition = new Vector3(0f, 1.6f, 0f);
            bar.localScale = new Vector3(1f, 0.08f, 0.08f);
            Part(PrimitiveType.Cube, bar, Vector3.zero, Vector3.one, new Color(0.1f, 0.1f, 0.1f));
            Transform fill = Part(PrimitiveType.Cube, bar, Vector3.zero, new Vector3(1f, 1.2f, 1.2f), new Color(0.3f, 0.9f, 0.4f));
            var fillAnchor = new GameObject("Fill").transform;
            fillAnchor.SetParent(bar, false);
            fill.SetParent(fillAnchor, false);

            var tracer = new GameObject("Tracer").AddComponent<LineRenderer>();
            tracer.transform.SetParent(root, false);
            tracer.positionCount = 2;
            tracer.startWidth = tracer.endWidth = 0.05f;
            tracer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit")) { color = new Color(1f, 0.85f, 0.4f) };
            tracer.enabled = false;

            return new TurretVisual { Root = root, Head = head, HealthFill = fillAnchor, Tracer = tracer };
        }

        private static Transform Part(PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            Destroy(part.GetComponent<Collider>());
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;
            part.GetComponent<Renderer>().material.color = color;
            return part.transform;
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
        }
    }
}
