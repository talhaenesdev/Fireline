using UnityEngine;
using FireLine.Scripts.Core.Damage;

namespace FireLine.Scripts.Core.Entity
{
    public abstract class Entity : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private int maxHealth = 100;

        private float _currentHealth;

        public float CurrentHealth => _currentHealth;
        public int MaxHealth => maxHealth;

        protected virtual void Awake()
        {
            _currentHealth = maxHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            if (damage <= 0)
                return;

            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }
        }

        protected virtual void Die()
        {
            Destroy(gameObject);
        }

    }
}