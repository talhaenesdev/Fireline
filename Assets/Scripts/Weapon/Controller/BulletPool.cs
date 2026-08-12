using UnityEngine;
using FireLine.Scripts.Weapon.View;
using FireLine.Scripts.Pooling;

namespace FireLine.Scripts.Weapon.Controller
{
    public class BulletPool
    {
        private readonly ObjectPool<BulletView> _pool;

        public BulletPool(
            BulletView prefab,
            Transform parent)
        {
            _pool = new ObjectPool<BulletView>(
                prefab,
                parent,
                20
            );
        }

        public BulletView Get()
        {
            return _pool.Get();
        }

        public void Release(BulletView bullet)
        {
            _pool.Release(bullet);
        }
    }
}