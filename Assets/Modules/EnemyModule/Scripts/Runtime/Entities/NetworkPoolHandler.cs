using FlowIoC.PoolModule.Entities;
using FlowIoC.PoolModule.Services;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>
    /// Lets Netcode spawn a prefab from the pool and despawn it back there, on the server and on every
    /// client alike, instead of instantiating and destroying one each time.
    /// </summary>
    internal class NetworkPoolHandler : INetworkPrefabInstanceHandler
    {
        private readonly IPoolService _pool;
        private readonly string _key;

        public NetworkPoolHandler(IPoolService pool, string key)
        {
            _pool = pool;
            _key = key;
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            PooledNetworkObject item = _pool.Get<PooledNetworkObject>(_key);
            item.transform.SetPositionAndRotation(position, rotation);
            return item.GetComponent<NetworkObject>();
        }

        public void Destroy(NetworkObject networkObject)
        {
            if (networkObject != null && networkObject.TryGetComponent(out PooledNetworkObject item))
                item.Dismiss();
        }
    }
}
