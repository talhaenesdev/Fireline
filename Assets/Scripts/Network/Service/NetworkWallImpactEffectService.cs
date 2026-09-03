using FireLine.Scripts.Pooling;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network.Service
{
    public class NetworkWallImpactEffectService :
        NetworkBehaviour
    {
        private IPoolService _poolService;

        [Inject]
        public void Construct(
            IPoolService poolService)
        {
            _poolService = poolService;
        }

        public void PlayWallImpact(
            Vector3 position)
        {
            if (!IsServer)
                return;

            PlayWallImpactClientRpc(
                position
            );
        }

        [ClientRpc]
        private void PlayWallImpactClientRpc(
            Vector3 position)
        {
            if (_poolService == null)
            {
                Debug.LogError(
                    "[WALL IMPACT FX] " +
                    "PoolService is NULL!"
                );

                return;
            }

            PooledParticleEffect effect =
                _poolService.Spawn<PooledParticleEffect>(
                    "WallImpact",
                    position,
                    Quaternion.identity
                );

            if (effect == null)
            {
                Debug.LogError(
                    "[WALL IMPACT FX] " +
                    "Could NOT spawn WallImpact!"
                );

                return;
            }

            Debug.Log(
                $"[WALL IMPACT FX] Spawned | " +
                $"Position: {position}"
            );
        }
    }
}