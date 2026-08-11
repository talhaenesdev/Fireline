using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerModel
    {
        public float MoveSpeed { get; private set; }
        public int MaxHealth { get; private set; }
        public int CurrentHealth { get; private set; }

        public PlayerModel(float moveSpeed, int maxHealth)
        {
            MoveSpeed = moveSpeed;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0)
                return;

            CurrentHealth -= damage;

            if (CurrentHealth < 0)
                CurrentHealth = 0;
        }

        public bool IsDead()
        {
            return CurrentHealth <= 0;
        }
    }

}
