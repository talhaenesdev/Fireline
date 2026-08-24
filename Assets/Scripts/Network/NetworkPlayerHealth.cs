using FireLine.Scripts.Network.Signals;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network
{
    public class NetworkPlayerHealth : NetworkBehaviour
    {
        [Header("Health")]
        [SerializeField]
        private float maxHealth = 100f;

        private readonly NetworkVariable<float> _currentHealth =
            new NetworkVariable<float>(
                100f,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );

        private readonly NetworkVariable<bool> _isDead =
            new NetworkVariable<bool>(
                false,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );

        private SignalBus _signalBus;

        public float CurrentHealth =>
            _currentHealth.Value;

        public float MaxHealth =>
            maxHealth;

        public bool IsDead =>
            _isDead.Value;

        [Inject]
        public void Construct(
            SignalBus signalBus)
        {
            _signalBus = signalBus;

            Debug.Log(
                $"[NETWORK HEALTH] " +
                $"SignalBus injected | {name}"
            );
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                _currentHealth.Value =
                    maxHealth;

                _isDead.Value =
                    false;
            }

            _currentHealth.OnValueChanged +=
                OnHealthChanged;

            _isDead.OnValueChanged +=
                OnDeathStateChanged;

            Debug.Log(
                $"[NETWORK HEALTH] Spawned | " +
                $"ClientId: {OwnerClientId} | " +
                $"IsServer: {IsServer} | " +
                $"Health: {_currentHealth.Value} | " +
                $"Dead: {_isDead.Value}"
            );
        }

        private void OnHealthChanged(
            float previous,
            float current)
        {
            Debug.Log(
                $"[NETWORK HEALTH] " +
                $"Health: {previous} -> {current} | " +
                $"ClientId: {OwnerClientId}"
            );
        }

        private void OnDeathStateChanged(
            bool previous,
            bool current)
        {
            Debug.Log(
                $"[NETWORK HEALTH] " +
                $"Death State: {previous} -> {current} | " +
                $"ClientId: {OwnerClientId}"
            );

            if (!current)
                return;

            if (!IsServer)
                return;

            if (_signalBus == null)
            {
                Debug.LogError(
                    "[NETWORK HEALTH] " +
                    "SignalBus is NULL!"
                );

                return;
            }

            Debug.Log(
                $"[NETWORK HEALTH] " +
                $"Firing NetworkPlayerDeathSignal | " +
                $"ClientId: {OwnerClientId}"
            );

            _signalBus.Fire(
                new NetworkPlayerDeathSignal(
                    OwnerClientId
                )
            );
        }

        public void TakeDamageServer(
            float damage)
        {
            if (!IsServer)
                return;

            if (damage <= 0f)
                return;

            if (_isDead.Value)
            {
                Debug.Log(
                    $"[NETWORK HEALTH] " +
                    $"Damage ignored. Player already dead | " +
                    $"ClientId: {OwnerClientId}"
                );

                return;
            }

            _currentHealth.Value -=
                damage;

            if (_currentHealth.Value <= 0f)
            {
                _currentHealth.Value = 0f;

                _isDead.Value = true;

                Debug.Log(
                    $"NETWORK PLAYER DEAD | " +
                    $"ClientId: {OwnerClientId}"
                );
            }
        }

        public void ResetHealthServer()
        {
            if (!IsServer)
                return;

            _currentHealth.Value =
                maxHealth;

            _isDead.Value =
                false;

            Debug.Log(
                $"[NETWORK HEALTH] Reset | " +
                $"ClientId: {OwnerClientId} | " +
                $"Health: {_currentHealth.Value} | " +
                $"Dead: {_isDead.Value}"
            );
        }

        public override void OnNetworkDespawn()
        {
            _currentHealth.OnValueChanged -=
                OnHealthChanged;

            _isDead.OnValueChanged -=
                OnDeathStateChanged;

            base.OnNetworkDespawn();
        }
    }
}