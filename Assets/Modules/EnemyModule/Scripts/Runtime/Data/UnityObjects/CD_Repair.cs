using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>Mending a damaged structure: how close the player stands, how much one press mends, and what it costs.</summary>
    [CreateAssetMenu(fileName = "CD_Repair", menuName = "Fireline/Enemy/CD_Repair")]
    public class CD_Repair : ScriptableObject
    {
        [Tooltip("The damaged structure nearest the player within this distance is the one mended.")]
        [SerializeField] private float _range = 3.5f;
        [Tooltip("Health one press gives back, at most.")]
        [SerializeField] private int _healthPerPress = 150;
        [Tooltip("Money one point of health costs; a press the player cannot pay in full mends what they can afford.")]
        [SerializeField] private float _costPerHealth = 0.2f;

        public float Range => _range;
        public int HealthPerPress => _healthPerPress;
        public float CostPerHealth => _costPerHealth;
    }
}
