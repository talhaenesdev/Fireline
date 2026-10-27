using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using Modules.EnemyModule.Data.UnityObjects;
using UnityEngine;

namespace Modules.EnemyModule.ViewsMediators
{
    /// <summary>The match scene's zombie wave settings, filed under EnemySystemRoot.</summary>
    [RequireComponent(typeof(ViewInjector))]
    public class ZombieWavesView : MonoBehaviour, IView
    {
        [SerializeField] private CD_ZombieWaves _config;

        public bool IsRegistered { get; set; }

        public CD_ZombieWaves Config => _config;
    }
}
