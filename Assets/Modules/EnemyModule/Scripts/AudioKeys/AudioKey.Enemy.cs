// ReSharper disable once CheckNamespace
namespace Modules.AudioModule.Shared
{
    /// <summary>
    /// The sounds EnemyModule plays. Compiled into Modules.Audio.Shared by the asmref beside it;
    /// every id starts with "EnemyModule/", the bank at EnemyModule/Resources/Audio/CD_AudioBank.asset.
    /// </summary>
    public readonly partial struct AudioKey
    {
        public static class Enemy
        {
            public static readonly AudioKey ZombieHit = new("EnemyModule/ZombieHit");
            public static readonly AudioKey ZombieDeath = new("EnemyModule/ZombieDeath");
            public static readonly AudioKey Explosion = new("EnemyModule/Explosion");
            public static readonly AudioKey StructureBreak = new("EnemyModule/StructureBreak");
            public static readonly AudioKey WaveCleared = new("EnemyModule/WaveCleared");
        }
    }
}
