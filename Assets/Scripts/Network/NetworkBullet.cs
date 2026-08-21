using FireLine.Scripts.Weapon.Model;
using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class NetworkBullet : NetworkBehaviour
    {
        [SerializeField]
        private BulletData bulletData;

        private Vector3 _direction;
        private float _remainingLifetime;

        public void Initialize(Vector3 direction)
        {
            if (!IsServer)
                return;

            _direction = direction.normalized;

            _remainingLifetime =
                bulletData.Lifetime;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log(
                $"[NETWORK BULLET SPAWN] " +
                $"Server: {IsServer} | " +
                $"Client: {IsClient}"
            );
        }

        private void Update()
        {
            if (!IsServer)
                return;

            transform.position +=
                _direction *
                bulletData.Speed *
                Time.deltaTime;

            _remainingLifetime -=
                Time.deltaTime;

            if (_remainingLifetime <= 0f)
            {
                NetworkObject.Despawn();
            }
        }
    }
}