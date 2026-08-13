using FireLine.Scripts.Core.Combat;
using FireLine.Scripts.Core.Damage;
using FireLine.Scripts.Core.Services.Entities;
using FireLine.Scripts.Pooling;
using FireLine.Scripts.Weapon.Model;
using UnityEngine;

namespace FireLine.Scripts.Weapon.View
{
    public class BulletView : MonoBehaviour, IPoolable, IDamageSource
    {
        private BulletData _bulletData;
        private IPoolService _poolService;
        private Entity _owner;
        private Vector3 _direction;
        private float _remainingLifetime;

        public Entity Owner => _owner;

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
            IDamageable damageable =
                other.GetComponentInParent<IDamageable>();

            if (damageable == null)
                return;

            Entity target =
                other.GetComponentInParent<Entity>();

            if (target == null)
                return;

            if (target == _owner)
                return;

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
            _owner = null;
        }

        public void OnDespawn()
        {
            _owner = null;
            _poolService = null;
            _direction = Vector3.zero;
            _remainingLifetime = 0f;
        }
    }
}