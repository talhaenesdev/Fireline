using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>A barricade a player buys: how much it takes, and how close a zombie must come before it is in the way.</summary>
    [CreateAssetMenu(fileName = "CD_Barricade", menuName = "Fireline/Enemy/CD_Barricade")]
    public class CD_Barricade : ScriptableObject
    {
        [SerializeField] private int _health = 250;
        [Tooltip("A zombie this close to a barricade stops and breaks it before going on.")]
        [SerializeField] private float _blockRadius = 1.6f;
        [SerializeField] private float _maxPlaceDistance = 8f;

        public int Health => _health;
        public float BlockRadius => _blockRadius;
        public float MaxPlaceDistance => _maxPlaceDistance;
    }
}
