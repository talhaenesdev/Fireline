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
                $"[NETWORK PLAYER] OnNetworkSpawn | " +
                $"Name: {name} | " +
                $"ClientId: {OwnerClientId} | " +
                $"IsOwner: {IsOwner} | " +
                $"IsServer: {IsServer} | " +
                $"IsClient: {IsClient}"
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

            if (targetPlayer == null)
                return;

            if (targetPlayer.OwnerClientId ==
                _ownerClientId)
            {
                return;
            }

            NetworkPlayerHealth health =
                targetPlayer.GetComponent<NetworkPlayerHealth>();

            if (health == null)
            {
                Debug.LogError(
                    $"NetworkPlayerHealth missing on " +
                    $"{targetPlayer.name}"
                );

                return;
            }

            Debug.Log(
                $"NETWORK BULLET HIT PLAYER | " +
                $"Target: {targetPlayer.OwnerClientId} | " +
                $"Damage: {damage}"
            );

            health.TakeDamageServer(damage);

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