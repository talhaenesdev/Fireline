using UnityEngine;

namespace Modules.EconomyModule.Data.UnityObjects
{
    /// <summary>
    /// Experience across matches: what a kill and a cleared wave are worth, how much each level takes,
    /// and the money a player starts a match with for every level they have.
    /// </summary>
    [CreateAssetMenu(fileName = "CD_Progression", menuName = "Fireline/Economy/CD_Progression")]
    public class CD_Progression : ScriptableObject
    {
        [SerializeField] private int _xpPerKill = 10;
        [SerializeField] private int _xpPerWave = 30;
        [Tooltip("Experience the first level takes; each level after takes this much more than the one before.")]
        [SerializeField] private int _xpPerLevel = 200;
        [SerializeField] private int _startingMoneyPerLevel = 15;

        public int XpPerKill => _xpPerKill;
        public int XpPerWave => _xpPerWave;
        public int StartingMoneyPerLevel => _startingMoneyPerLevel;

        /// <summary>The level a total of experience reaches, and how far into the next it is.</summary>
        public int LevelOf(int totalXp, out int intoLevel, out int levelSize)
        {
            int level = 0;
            levelSize = _xpPerLevel;
            intoLevel = Mathf.Max(0, totalXp);
            while (intoLevel >= levelSize && levelSize > 0)
            {
                intoLevel -= levelSize;
                level++;
                levelSize = _xpPerLevel * (level + 1);
            }

            return level;
        }
    }
}
