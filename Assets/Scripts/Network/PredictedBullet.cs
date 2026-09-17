using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class PredictedBullet : MonoBehaviour
    {
        private Vector3 _direction;
        private float _speed;
        private float _remainingLifetime;

        private Transform _ownerTransform;

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
                {
                    continue;
                }

                Debug.Log(
                    $"[PREDICTED BULLET] " +
                    $"Hit={hit.collider.name} | " +
                    $"Position={hit.point}"
                );

                transform.position =
                    hit.point;

                Destroy(gameObject);

                return;
            }

            transform.position +=
                movement;

            _remainingLifetime -=
                Time.deltaTime;

            if (_remainingLifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}