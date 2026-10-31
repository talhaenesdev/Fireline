using System.Collections.Generic;
using FlowIoC.ConsoleModule;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Data.ValueObjects;
using UnityEngine;

namespace Modules.EnemyModule.Controllers
{
    /// <summary>
    /// The zombies one wave brings, one entry per zombie, shuffled: the wave's row of the table, or past
    /// the table the last row grown by the configured share per extra wave, with a boss every few waves
    /// and a rising elite chance. An elite comes as its own copy of the kind. A group naming a kind the
    /// config does not have is reported and left out.
    /// </summary>
    internal class ComposeWaveFunction
    {
        public List<ZombieTypeCVO> Execute(CD_ZombieWaves config, int wave, int difficulty)
        {
            var zombies = new List<ZombieTypeCVO>();
            if (config.Waves.Count == 0 || config.Types.Count == 0)
                return zombies;

            int row = Mathf.Min(wave, config.Waves.Count) - 1;
            int extraWaves = wave - 1 - row;
            ZombieDifficultyCVO level = config.Difficulty(difficulty);
            float growth = (1f + config.GrowthAfterLastWave * extraWaves) * level.CountMultiplier;
            float extraElite = config.EliteChancePerExtraWave * extraWaves;

            foreach (ZombieWaveGroupCVO group in config.Waves[row].Groups)
                AddGroup(config, zombies, group.Type, Mathf.Max(group.Count > 0 ? 1 : 0, Mathf.RoundToInt(group.Count * growth)), group.EliteChance + extraElite, row + 1);

            if (extraWaves > 0 && config.BossEveryWaves > 0 && wave % config.BossEveryWaves == 0)
                AddGroup(config, zombies, config.BossType, 1, extraElite, wave);

            for (int i = 0; i < zombies.Count; i++)
                zombies[i] = level.Apply(zombies[i]);

            for (int i = zombies.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (zombies[i], zombies[j]) = (zombies[j], zombies[i]);
            }

            return zombies;
        }

        private static void AddGroup(CD_ZombieWaves config, List<ZombieTypeCVO> zombies, string name, int count, float eliteChance, int wave)
        {
            ZombieTypeCVO type = Find(config, name);
            if (type == null)
            {
                FlowLogger.LogError($"ComposeWaveFunction - wave {wave} asks for zombie kind '{name}', which CD_ZombieWaves does not list.");
                return;
            }

            for (int i = 0; i < count; i++)
                zombies.Add(Random.value < eliteChance ? config.Elite.Apply(type) : type);
        }

        private static ZombieTypeCVO Find(CD_ZombieWaves config, string name)
        {
            foreach (ZombieTypeCVO type in config.Types)
            {
                if (type.Name == name)
                    return type;
            }

            return null;
        }
    }
}
