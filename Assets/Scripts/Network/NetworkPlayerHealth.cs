using FireLine.Scripts.Network.Signals;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network
{
    public class NetworkPlayerHealth : NetworkBehaviour
    {
        [SerializeField]
        private float maxHealth = 100f;

        private SignalBus _signalBus;

        private readonly NetworkVariable<float> _health =
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

        public float CurrentHealth =>
            _health.Value;

        public bool IsDead =>
            _isDead.Value;

        public event System.Action<bool>
            DeathStateChanged;

        [Inject]
        public void Construct(
            SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _health.OnValueChanged +=
                OnHealthChanged;

            _isDead.OnValueChanged +=
                OnDeathStateChanged;

            if (IsServer)
            {
                _health.Value =
                    maxHealth;

                _isDead.Value =
                    false;
            }

            Debug.Log(
                $"[NETWORK HEALTH] Spawned | " +
                $"ClientId: {OwnerClientId} | " +
                $"Health: {_health.Value}"
            );
        }

        private void OnHealthChanged(
            float previous,
            float current)
        {
            Debug.Log(
                $"[NETWORK HEALTH] " +
                $"{previous} -> {current}"
            );
        }

        private void OnDeathStateChanged(
            bool previous,
            bool current)
        {
            DeathStateChanged?.Invoke(
                current
            );

            Debug.Log(
                $"[NETWORK HEALTH] " +
                $"Death State: " +
                $"{previous} -> {current}"
            );
        }

        public void TakeDamageServer(
            int damage,
            ulong attackerClientId)
        {
            if (!IsServer)
                return;

            if (_isDead.Value)
                return;

            if (damage <= 0)
                return;

            _health.Value -= damage;

            Debug.Log(
                $"[NETWORK HEALTH] Damage | " +
                $"Victim: {OwnerClientId} | " +
                $"Attacker: {attackerClientId} | " +
                $"Damage: {damage} | " +
                $"Health: {_health.Value}"
            );

            if (_health.Value <= 0)
            {
                _health.Value = 0;

                Die(attackerClientId);
            }
        }

        private void Die(
            ulong killerClientId)
        {
            if (!IsServer)
                return;

            if (_isDead.Value)
                return;

            _isDead.Value = true;

            Debug.Log(
                $"NETWORK PLAYER DEAD | " +
                $"Victim: {OwnerClientId} | " +
                $"Killer: {killerClientId}"
            );

            if (_signalBus == null)
            {
                Debug.LogError(
                    "[NETWORK HEALTH] " +
                    "SignalBus is NULL!"
                );

                return;
            }

            _signalBus.Fire(
                new NetworkPlayerDeathSignal(
                    OwnerClientId,
                    killerClientId
                )
            );
        }

        public override void OnNetworkDespawn()
        {
            _health.OnValueChanged -=
                OnHealthChanged;

            _isDead.OnValueChanged -=
                OnDeathStateChanged;

            DeathStateChanged = null;

            base.OnNetworkDespawn();
        }
    }
}