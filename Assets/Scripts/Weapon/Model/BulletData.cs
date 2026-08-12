using UnityEngine;

namespace FireLine.Scripts.Weapon.Model
{
    [CreateAssetMenu(
        fileName = "BulletData",
        menuName = "FireLine/Weapon/Bullet Data"
    )]
    public class BulletData : ScriptableObject
    {
        [Header("Pooling")]
        [SerializeField] private string poolKey;

        [Header("Stats")]
        [SerializeField] private float speed = 20f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifetime = 3f;

        public string PoolKey => poolKey;

        public float Speed => speed;
        public float Damage => damage;
        public float Lifetime => lifetime;
    }
}