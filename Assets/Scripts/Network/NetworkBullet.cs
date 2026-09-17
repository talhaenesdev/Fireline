using FireLine.Scripts.Network.Service;
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

            _direction =
                direction.normalized;

            _ownerClientId =
                ownerClientId;

            _remainingLifetime =
                lifetime;

            Debug.Log(
                $"[NETWORK BULLET DEBUG] SERVER INIT | " +
                $"Position={transform.position} | " +
                $"Layer={LayerMask.LayerToName(gameObject.layer)} | " +
                $"Collider={GetComponent<Collider>() != null} | " +
                $"Rigidbody={GetComponent<Rigidbody>() != null}"
            );
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                HideVisualForOwner();
            }

            Debug.Log(
                $"[NETWORK BULLET DEBUG] " +
                $"Layer={gameObject.layer} | " +
                $"LayerName={LayerMask.LayerToName(gameObject.layer)} | " +
                $"Collider={GetComponent<Collider>() != null} | " +
                $"Rigidbody={GetComponent<Rigidbody>() != null}"
            );
        }

        private void HideVisualForOwner()
        {
            Renderer[] renderers =
                GetComponentsInChildren<Renderer>(
                    true
                );

            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }

            Debug.Log(
                "[NETWORK BULLET] " +
                "Visual hidden for owner."
            );
        }

        private void Update()
        {
            if (!IsServer)
                return;

            Debug.Log(
                $"[NETWORK BULLET UPDATE] " +
                $"ID={NetworkObjectId} | " +
                $"Pos={transform.position} | " +
                $"Direction={_direction} | " +
                $"Speed={speed}"
            );

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

            Debug.Log(
    $"[NETWORK BULLET] SERVER COLLISION | " +
    $"Bullet={name} | " +
    $"Other={other.name} | " +
    $"Position={transform.position}"
);

            NetworkPlayer targetPlayer =
                other.GetComponentInParent<NetworkPlayer>();

            // PLAYER HIT
            if (targetPlayer != null)
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

                return;
            }

            // WALL / OTHER COLLIDER HIT
            Debug.Log(
                $"[NETWORK BULLET] HIT WALL | " +
                $"Collider: {other.name} | " +
                $"Position: {transform.position}"
            );

            PlayWallImpact();

            Despawn();
        }

        private void PlayWallImpact()
        {
            NetworkWallImpactEffectService service =
                FindFirstObjectByType<
                    NetworkWallImpactEffectService>();

            if (service == null)
            {
                Debug.LogError(
                    "[NETWORK BULLET] " +
                    "NetworkWallImpactEffectService " +
                    "NOT FOUND!"
                );

                return;
            }

            service.PlayWallImpact(
                transform.position
            );
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