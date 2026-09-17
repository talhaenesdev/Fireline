using FireLine.Scripts.Pooling;
using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class PredictedBullet :
        MonoBehaviour,
        Pooling.IPoolable
    {
        private string _poolKey;

        private IPoolService _poolService;

        private Vector3 _direction;
        private float _speed;
        private float _remainingLifetime;

        private Transform _ownerTransform;

        public void Initialize(
            IPoolService poolService,
            string poolKey)
        {
            _poolService =
                poolService;

            _poolKey =
                poolKey;
        }

        public void Initialize(
            Vector3 direction,
            float speed,
            float lifetime,
            Transform ownerTransform)
        {
            _direction =
                direction.normalized;

            _speed =
                speed;

            _remainingLifetime =
                lifetime;

            _ownerTransform =
                ownerTransform;
        }

        public void OnSpawn()
        {
            _direction =
                Vector3.zero;

            _speed =
                0f;

            _remainingLifetime =
                0f;

            _ownerTransform =
                null;
        }

        public void OnDespawn()
        {
            _direction =
                Vector3.zero;

            _speed =
                0f;

            _remainingLifetime =
                0f;

            _ownerTransform =
                null;
        }

        private void Update()
        {
            float movementDistance =
                _speed *
                Time.deltaTime;

            Vector3 startPosition =
                transform.position;

            Vector3 movement =
                _direction *
                movementDistance;

            RaycastHit[] hits =
                Physics.RaycastAll(
                    startPosition,
                    _direction,
                    movementDistance
                );

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == null)
                    continue;

                if (_ownerTransform != null &&
                    hit.collider.transform.IsChildOf(
                        _ownerTransform))
                {
                    continue;
                }

                NetworkBullet networkBullet =
                    hit.collider.GetComponentInParent<
                        NetworkBullet>();

                if (networkBullet != null)
                    continue;

                Debug.Log(
                    $"[PREDICTED BULLET] " +
                    $"Hit={hit.collider.name} | " +
                    $"Position={hit.point}"
                );

                transform.position =
                    hit.point;

                Despawn();

                return;
            }

            transform.position +=
                movement;

            _remainingLifetime -=
                Time.deltaTime;

            if (_remainingLifetime <= 0f)
            {
                Despawn();
            }
        }

        private void Despawn()
        {
            if (_poolService == null)
            {
                Debug.LogError(
                    "[PREDICTED BULLET] " +
                    "PoolService is NULL!"
                );

                return;
            }

            _poolService.Despawn(
                _poolKey,
                this
            );
        }
    }
}