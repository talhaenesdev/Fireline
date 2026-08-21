using FireLine.Scripts.Core.Damage;
using FireLine.Scripts.Core.Services.Entities;
using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class NetworkBullet : NetworkBehaviour
    {
        [SerializeField]
        private float speed = 20f;

        [SerializeField]
        private float lifetime = 3f;

        [SerializeField]
        private int damage = 10;

        private Vector3 _direction;
        private float _remainingLifetime;
        private ulong _ownerClientId;

        public void Initialize(
            Vector3 direction,
            ulong ownerClientId)
        {
            if (!IsServer)
                return;

            _direction = direction.normalized;
            _ownerClientId = ownerClientId;

            _remainingLifetime = lifetime;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log(
                $"NetworkBullet Spawned | " +
                $"Id: {NetworkObjectId} | " +
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
                speed *
                Time.deltaTime;

            _remainingLifetime -=
                Time.deltaTime;

            if (_remainingLifetime <= 0f)
            {
                Despawn();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
                return;

            NetworkPlayer targetPlayer =
                other.GetComponentInParent<NetworkPlayer>();

            if (targetPlayer != null)
            {
                if (targetPlayer.OwnerClientId ==
                    _ownerClientId)
                {
                    return;
                }
            }

            IDamageable damageable =
                other.GetComponentInParent<IDamageable>();

            if (damageable == null)
                return;

            Entity target =
                other.GetComponentInParent<Entity>();

            if (target == null)
                return;

            Debug.Log(
                $"NETWORK BULLET HIT: {target.name}"
            );

            damageable.TakeDamage(damage);

            Despawn();
        }

        private void Despawn()
        {
            if (!IsServer)
                return;

            if (NetworkObject.IsSpawned)
            {
                NetworkObject.Despawn();
            }
        }
    }
}