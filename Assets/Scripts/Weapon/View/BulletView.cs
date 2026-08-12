using UnityEngine;
using FireLine.Scripts.Pooling;
using FireLine.Scripts.Weapon.Model;

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
            if (_bulletData != null)
            {
                _remainingLifetime =
                    _bulletData.Lifetime;
            }
        }

        public void OnDespawn()
        {
            _bulletData = null;
            _poolService = null;
            _direction = Vector3.zero;
            _remainingLifetime = 0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            Despawn();
        }
    }
}