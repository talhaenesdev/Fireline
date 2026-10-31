using System;
using System.Collections.Generic;

namespace Modules.EnemyModule.Data.ValueObjects
{
    /// <summary>How many of one kind of zombie a wave brings.</summary>
    [Serializable]
    public class ZombieWaveGroupCVO
    {
        public string Type = "Walker";
        public int Count = 3;
        [UnityEngine.Tooltip("The chance each zombie of the group comes as its elite, 0 to 1.")]
        [UnityEngine.Range(0f, 1f)] public float EliteChance;
    }

    /// <summary>One wave: every group in it comes in together, shuffled.</summary>
    [Serializable]
    public class ZombieWaveCVO
    {
        public List<ZombieWaveGroupCVO> Groups = new();
    }
}
