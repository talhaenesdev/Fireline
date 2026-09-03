using Unity.Netcode;
using UnityEngine;
using Zenject;

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

        [SerializeField]
        private string wallImpactPoolKey = "WallImpact";

        private Vector3 _direction;
        private float _remainingLifetime;
        private ulong _ownerClientId;


        public void Initialize(
            Vector3 direction,
            ulong ownerClientId)
        {
            if (!IsServer)
                return;

            _direction =
                direction.normalized;

            _ownerClientId =
                ownerClientId;

            _remainingLifetime =
                lifetime;

            Debug.Log(
                $"[NETWORK BULLET] Initialize | " +
                $"NetworkObjectId: {NetworkObjectId} | " +
                $"OwnerClientId: {_ownerClientId} | " +
                $"Direction: {_direction}"
            );
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log(
                $"[NETWORK BULLET] OnNetworkSpawn | " +
                $"Name: {name} | " +
                $"NetworkObjectId: {NetworkObjectId} | " +
                $"OwnerClientId: {OwnerClientId} | " +
                $"IsOwner: {IsOwner} | " +
                $"IsServer: {IsServer} | " +
                $"IsClient: {IsClient} | " +
                $"Position: {transform.position}"
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

        private void OnTriggerEnter(
            Collider other)
        {
            if (!IsServer)
                return;

            NetworkPlayer targetPlayer =
                other.GetComponentInParent<NetworkPlayer>();

            if (targetPlayer != null)
            {
                HandlePlayerHit(targetPlayer);
                return;
            }

            HandleEnvironmentHit();
        }

        private void HandlePlayerHit(
            NetworkPlayer targetPlayer)
        {
            if (targetPlayer.OwnerClientId ==
                _ownerClientId)
            {
                return;
            }

            NetworkPlayerHealth health =
                targetPlayer.GetComponent<
                    NetworkPlayerHealth>();

            if (health == null)
            {
                Debug.LogError(
                    $"[NETWORK BULLET] " +
                    $"NetworkPlayerHealth missing on " +
                    $"{targetPlayer.name}"
                );

                return;
            }

            Debug.Log(
                $"[NETWORK BULLET] HIT PLAYER | " +
                $"Target: {targetPlayer.OwnerClientId} | " +
                $"Damage: {damage} | " +
                $"Attacker: {_ownerClientId}"
            );

            health.TakeDamageServer(
                damage,
                _ownerClientId
            );

            Despawn();
        }

        private void HandleEnvironmentHit()
        {
            Vector3 impactPosition =
                transform.position;

            Debug.Log(
                $"[NETWORK BULLET] " +
                $"HIT ENVIRONMENT | " +
                $"Position: {impactPosition}"
            );

            Despawn();
        }

        
        private void Despawn()
        {
            if (!IsServer)
                return;

            if (NetworkObject != null &&
                NetworkObject.IsSpawned)
            {
                NetworkObject.Despawn();
            }
        }
    }
}