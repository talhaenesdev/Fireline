using FireLine.Scripts.Core.Damage;
using FireLine.Scripts.Core.Services;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Core.Entities
{
    public abstract class Entity : MonoBehaviour, IDamageable
    {
        private IEntityDeathService _entityDeathService;

        [Inject]
        public void Construct(IEntityDeathService entityDeathService)
        {
            _entityDeathService = entityDeathService;
        }
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
            _entityDeathService.HandleDeath(this);
        }
    }
}