using FireLine.Scripts.Core.Services.Entities;
using FireLine.Scripts.Core.Signals;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network
{
    public class NetworkEntityDeathHandler : MonoBehaviour, IInitializable
    {
        private readonly SignalBus _signalBus;

        public NetworkEntityDeathHandler(
            SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<EntityDestroyedSignal>(
                OnEntityDestroyed
            );

            Debug.Log(
                "[NETWORK DEATH] Subscribed."
            );
        }

        private void OnEntityDestroyed(
            EntityDestroyedSignal signal)
        {
            if (signal.Entity is not Entity entity)
                return;

            NetworkObject networkObject =
                entity.GetComponent<NetworkObject>();

            if (networkObject == null)
                return;

            if (!networkObject.IsSpawned)
                return;

            Debug.Log(
                $"[NETWORK DEATH] " +
                $"Despawning: {entity.name}"
            );

            networkObject.Despawn();
        }
    }
}