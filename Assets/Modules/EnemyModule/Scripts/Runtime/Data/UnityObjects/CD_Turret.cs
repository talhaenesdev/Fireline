using System.Collections.Generic;
using Modules.EnemyModule.Data.ValueObjects;
using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>A turret a player buys: its levels, each one an upgrade, and how far from the player it may go down.</summary>
    [CreateAssetMenu(fileName = "CD_Turret", menuName = "Fireline/Enemy/CD_Turret")]
    public class CD_Turret : ScriptableObject
    {
        [Tooltip("Element 0 is what a new turret is; each upgrade moves it one element on.")]
        [SerializeField] private List<TurretLevelCVO> _levels = new() { new TurretLevelCVO() };
        [Tooltip("How far from the player the server still accepts the spot it is put down at.")]
        [SerializeField] private float _maxPlaceDistance = 8f;
        [Tooltip("The colour of the turret's head at each level, so an upgraded one shows.")]
        [SerializeField] private List<Color> _levelColors = new() { new Color(0.35f, 0.55f, 0.85f) };

        public int LevelCount => Mathf.Max(1, _levels.Count);
        public TurretLevelCVO Level(int level) => _levels.Count == 0 ? new TurretLevelCVO() : _levels[Mathf.Clamp(level, 0, _levels.Count - 1)];
        public Color LevelColor(int level) => _levelColors.Count == 0 ? Color.white : _levelColors[Mathf.Clamp(level, 0, _levelColors.Count - 1)];
        public float MaxPlaceDistance => _maxPlaceDistance;
    }
}
