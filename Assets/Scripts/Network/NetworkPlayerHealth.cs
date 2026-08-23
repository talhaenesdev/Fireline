using Unity.Netcode;
using UnityEngine;
using System;

namespace FireLine.Scripts.Network
{
    public class NetworkPlayerHealth : NetworkBehaviour
    {

        private NetworkVariable<float> _health =
            new NetworkVariable<float>(
                100f,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );

        private bool _deathTriggered;

        public float Health =>
            _health.Value;

        public event Action OnDeath;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _health.OnValueChanged +=
                OnHealthChanged;
        }

        private void OnHealthChanged(
            float previous,
            float current)
        {
            Debug.Log(
                $"NETWORK HEALTH | " +
                $"{previous} -> {current}"
            );

            if (!IsServer)
                return;

            if (current <= 0f &&
                !_deathTriggered)
            {
                _deathTriggered = true;

                Debug.Log(
                    $"[NETWORK HEALTH] Death triggered | " +
                    $"ClientId: {OwnerClientId}"
                );

                OnDeath?.Invoke();
            }
        }

        public void TakeDamageServer(
            float damage)
        {
            if (!IsServer)
                return;

            if (damage <= 0f)
                return;


            float newHealth =
                Mathf.Max(
                    0f,
                    _health.Value - damage
                );

            Debug.Log(
                $"NETWORK DAMAGE | " +
                $"{_health.Value} -> {newHealth}"
            );

            _health.Value = newHealth;
        }

        public override void OnNetworkDespawn()
        {
            _health.OnValueChanged -=
                OnHealthChanged;

            OnDeath = null;

            base.OnNetworkDespawn();
        }
    }
}