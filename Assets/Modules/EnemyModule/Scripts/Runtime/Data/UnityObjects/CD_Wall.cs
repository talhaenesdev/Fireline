using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>A wall a player buys, and the walls the base starts with: how much it takes, and how close a zombie must come before it is in the way.</summary>
    [CreateAssetMenu(fileName = "CD_Wall", menuName = "Fireline/Enemy/CD_Wall")]
    public class CD_Wall : ScriptableObject
    {
        [SerializeField] private int _health = 600;
        [Tooltip("A zombie this close to a wall stops and breaks it before going on.")]
        [SerializeField] private float _blockRadius = 2f;
        [SerializeField] private float _maxPlaceDistance = 8f;

        public int Health => _health;
        public float BlockRadius => _blockRadius;
        public float MaxPlaceDistance => _maxPlaceDistance;
    }
}
