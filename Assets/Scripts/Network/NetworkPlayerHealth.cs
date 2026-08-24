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

        public NetworkVariable<float> CurrentHealth =
            new NetworkVariable<float>(
                100f,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );

        [Inject]
        public void Construct(
            SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            CurrentHealth.OnValueChanged +=
                OnHealthChanged;

            if (IsServer)
            {
                CurrentHealth.Value = maxHealth;
            }

            Debug.Log(
                $"[NETWORK HEALTH] Spawned | " +
                $"ClientId: {OwnerClientId} | " +
                $"Health: {CurrentHealth.Value}"
            );
        }

        private void OnHealthChanged(
            float previous,
            float current)
        {
            Debug.Log(
                $"NETWORK HEALTH SYNC | " +
                $"{previous} -> {current}"
            );
        }

        public void TakeDamageServer(float damage)
        {
            if (!IsServer)
                return;

            if (damage <= 0f)
                return;

            if (CurrentHealth.Value <= 0f)
                return;

            float previousHealth =
                CurrentHealth.Value;

            CurrentHealth.Value =
                Mathf.Max(
                    0f,
                    CurrentHealth.Value - damage
                );

            Debug.Log(
                $"NETWORK HEALTH | " +
                $"{previousHealth} -> {CurrentHealth.Value}"
            );

            if (CurrentHealth.Value <= 0f)
            {
                Debug.Log(
                    $"NETWORK PLAYER DEAD | " +
                    $"ClientId: {OwnerClientId}"
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
                        OwnerClientId
                    )
                );
            }
        }

        public override void OnNetworkDespawn()
        {
            CurrentHealth.OnValueChanged -=
                OnHealthChanged;

            base.OnNetworkDespawn();
        }
    }
}