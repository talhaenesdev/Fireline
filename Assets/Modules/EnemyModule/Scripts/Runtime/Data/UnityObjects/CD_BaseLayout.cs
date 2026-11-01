using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>
    /// The base a map starts with: a square ring of walls around the board's base anchor, with a gate in
    /// the middle of every side. The walls themselves are CD_Wall's.
    /// </summary>
    [CreateAssetMenu(fileName = "CD_BaseLayout", menuName = "Fireline/Enemy/CD_BaseLayout")]
    public class CD_BaseLayout : ScriptableObject
    {
        [Tooltip("Half the side of the square, from the anchor to a wall.")]
        [SerializeField] private float _halfSize = 14f;
        [Tooltip("Metres between two walls along a side.")]
        [SerializeField] private float _wallSpacing = 2.5f;
        [Tooltip("The opening left in the middle of every side.")]
        [SerializeField] private float _gateWidth = 4f;
        [SerializeField] private bool _cornerWalls = true;

        public float HalfSize => _halfSize;
        public float WallSpacing => _wallSpacing;
        public float GateWidth => _gateWidth;
        public bool CornerWalls => _cornerWalls;
    }
}
