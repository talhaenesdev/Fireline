using FireLine.Scripts.Core.Services.Entities;
using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class NetworkPlayerHealth : NetworkBehaviour
    {
        private Entity _entity;

        private NetworkVariable<float> _health =
            new NetworkVariable<float>(
                100f,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );

        public float Health =>
            _health.Value;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _entity = GetComponent<Entity>();

            if (_entity == null)
            {
                Debug.LogError(
                    $"Entity not found on {gameObject.name}"
                );

                return;
            }

            if (IsServer)
            {
                _health.Value = _entity.MaxHealth;
            }

            _health.OnValueChanged += OnHealthChanged;
        }

        private void OnHealthChanged(
            float previous,
            float current)
        {
            Debug.Log(
                $"NETWORK HEALTH | " +
                $"{previous} -> {current}"
            );
        }

        public void TakeDamageServer(float damage)
        {
            if (!IsServer)
                return;

            if (_entity == null)
                return;

            if (damage <= 0f)
                return;

            _entity.TakeDamage(damage);

            _health.Value =
                _entity.CurrentHealth;
        }

        public override void OnNetworkDespawn()
        {
            _health.OnValueChanged -=
                OnHealthChanged;

            base.OnNetworkDespawn();
        }
    }
}