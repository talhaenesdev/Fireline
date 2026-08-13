using UnityEngine;
using FireLine.Scripts.Pooling;
using FireLine.Scripts.Weapon.Model;
using FireLine.Scripts.Core.Damage;

namespace FireLine.Scripts.Weapon.View
{
    public class BulletView : MonoBehaviour, IPoolable
    {
        private BulletData _bulletData;
        private IPoolService _poolService;

        private Vector3 _direction;
        private float _remainingLifetime;

        public void Initialize(
            BulletData bulletData,
            Vector3 direction,
            IPoolService poolService)
        {
            Debug.Log(
    $"Bullet Initialize | Data: {bulletData?.name}"
);
            _bulletData = bulletData;
            _direction = direction.normalized;
            _poolService = poolService;

            _remainingLifetime =
                bulletData.Lifetime;

        }

        private void Update()
        {
            if (_bulletData == null)
                return;

            transform.position +=
                _direction *
                _bulletData.Speed *
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
            Debug.Log(
    $"Bullet Collision | Data: {_bulletData?.name}"
);
            IDamageable damageable =
                other.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                return;
            }

            if (_bulletData == null)
            {
                Debug.LogError("BulletData is NULL!");

                return;
            }

            damageable.TakeDamage(_bulletData.Damage);

            Despawn();
        }
        private void Despawn()
        {
            if (_bulletData == null ||
                _poolService == null)
            {
                return;
            }

            _poolService.Despawn(
                _bulletData.PoolKey,
                this
            );
        }

        public void OnSpawn()
        {
            // State Initialize tarafýndan ayarlanýyor.
        }

        public void OnDespawn()
        {
            //_bulletData = null;
            _poolService = null;
            _direction = Vector3.zero;
            _remainingLifetime = 0f;
        }
    }
}