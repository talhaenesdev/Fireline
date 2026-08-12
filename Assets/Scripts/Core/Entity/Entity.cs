using FireLine.Scripts.Core.Damage;
using UnityEngine;

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

        public virtual void TakeDamage(int damage)
        {
            if (damage <= 0)
                return;

            _currentHealth -= damage;

            Debug.Log(
                $"{gameObject.name} took {damage} damage. " +
                $"Health: {_currentHealth}/{maxHealth}"
            );

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }
        }

        protected virtual void Die()
        {
            Debug.Log($"{gameObject.name} died.");
        }
    }
}