using System;
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
    [RequireComponent(typeof(ViewInjector))]
    public class ZombieWavesView : NetworkBehaviour, IView, IZombieWaveBoard
    {
        [SerializeField] private CD_ZombieWaves _config;
        [SerializeField] private Transform _spawnPointRoot;

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

        public CD_ZombieWaves Config => _config;
        public int SpawnPointCount => _spawnPointRoot != null ? _spawnPointRoot.childCount : 0;
        public Vector3 GetSpawnPoint(int index) => _spawnPointRoot.GetChild(index).position;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _wave.OnValueChanged += OnIntChanged;
            _zombiesLeft.OnValueChanged += OnIntChanged;
            _nextWaveTime.OnValueChanged += OnTimeChanged;
            Publish();
        }

        public override void OnNetworkDespawn()
        {
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
