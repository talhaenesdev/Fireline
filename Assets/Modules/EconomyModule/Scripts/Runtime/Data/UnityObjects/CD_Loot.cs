using System.Collections.Generic;
using Modules.EconomyModule.Data.ValueObjects;
using UnityEngine;

namespace Modules.EconomyModule.Data.UnityObjects
{
    /// <summary>What zombies drop, how it is picked up and sold, and what a cleared wave pays everyone.</summary>
    [CreateAssetMenu(fileName = "CD_Loot", menuName = "Fireline/Economy/CD_Loot")]
    public class CD_Loot : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float _dropChance = 0.6f;
        [SerializeField] private float _pickupRadius = 1.6f;
        [SerializeField] private List<LootItemCVO> _items = new();

        [Header("Wave reward: paid to every player when a wave is cleared")]
        [SerializeField] private int _waveRewardBase = 25;
        [SerializeField] private int _waveRewardPerWave = 10;

        public float DropChance => _dropChance;
        public float PickupRadius => _pickupRadius;
        public IReadOnlyList<LootItemCVO> Items => _items;

        public int WaveReward(int wave) => _waveRewardBase + _waveRewardPerWave * Mathf.Max(0, wave - 1);
    }
}
