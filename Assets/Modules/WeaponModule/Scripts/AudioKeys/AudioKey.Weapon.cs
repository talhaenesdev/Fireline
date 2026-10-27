// ReSharper disable once CheckNamespace
namespace Modules.AudioModule.Shared
{
    /// <summary>
    /// The sounds WeaponModule plays. Compiled into Modules.Audio.Shared by the asmref beside it;
    /// every id starts with "WeaponModule/", the bank at WeaponModule/Resources/Audio/CD_AudioBank.asset.
    /// </summary>
    public readonly partial struct AudioKey
    {
        public static class Weapon
        {
            public static readonly AudioKey Fire = new("WeaponModule/Fire");
            public static readonly AudioKey Reload = new("WeaponModule/Reload");
            public static readonly AudioKey WallImpact = new("WeaponModule/WallImpact");
        }
    }
}
