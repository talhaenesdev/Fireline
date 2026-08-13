using FireLine.Scripts.Core.Damage;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Core.Services.Entities
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
        private bool _isDead;
        public float CurrentHealth => _currentHealth;
        public int MaxHealth => maxHealth;

        protected virtual void Awake()
        {
            _currentHealth = maxHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            if (_isDead)
                return;

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
                _isDead = true;

                Die();
            }
        }

        private void Die()
        {
            if (_entityDeathService == null)
            {
                Debug.LogError(
                    $"EntityDeathService is NULL on {gameObject.name}"
                );

                return;
            }

            _entityDeathService.HandleDeath(this);
        }
        protected void ResetHealth()
        {
            _currentHealth = maxHealth;
            _isDead = false;
        }
    }
}