using UnityEngine;
using FireLine.Scripts.Core.Pooling;

namespace FireLine.Scripts.Weapon.View
{
    public class BulletView : MonoBehaviour, IPoolable
    {
        private Vector3 _direction;
        private float _speed;

        public void Initialize(
            Vector3 direction,
            float speed)
        {
            _direction = direction.normalized;
            _speed = speed;
        }

        private void Update()
        {
            transform.position +=
                _direction *
                _speed *
                Time.deltaTime;
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
            _direction = Vector3.zero;
            _speed = 0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Þimdilik sadece pool'a döndüreceðiz.
        }
    }
}