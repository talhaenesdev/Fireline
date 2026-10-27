using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.WeaponModule.Entities
{
    /// <summary>
    /// The round the shooter sees at once, before the server's round arrives. It decides nothing:
    /// it flies, stops at the first thing that is not its shooter, and goes back to its pool.
    /// </summary>
    public class PredictedBullet : PoolableItem
    {
        private Vector3 _direction;
        private float _speed;
        private float _remainingLifetime;
        private Transform _shooter;

        public void Launch(Vector3 position, Vector3 direction, float speed, float lifetime, Transform shooter)
        {
            transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
            _direction = direction.normalized;
            _speed = speed;
            _remainingLifetime = lifetime;
            _shooter = shooter;

            if (TryGetComponent(out TrailRenderer trail))
                trail.Clear();
        }

        public override void OnReturnToPool()
        {
            _speed = 0f;
            _shooter = null;
        }

        private void Update()
        {
            float distance = _speed * Time.deltaTime;

            foreach (RaycastHit hit in Physics.RaycastAll(transform.position, _direction, distance))
            {
                if (hit.collider == null)
                    continue;

                if (_shooter != null && hit.collider.transform.IsChildOf(_shooter))
                    continue;

                // The server's round flies the same line; meeting it is not a hit.
                if (hit.collider.GetComponentInParent<Unity.Netcode.NetworkObject>() is { } networkObject
                    && networkObject.GetComponent<ViewsMediators.NetworkBulletView>() != null)
                    continue;

                transform.position = hit.point;
                Dismiss();
                return;
            }

            transform.position += _direction * distance;
            _remainingLifetime -= Time.deltaTime;

            if (_remainingLifetime <= 0f)
                Dismiss();
        }
    }
}
