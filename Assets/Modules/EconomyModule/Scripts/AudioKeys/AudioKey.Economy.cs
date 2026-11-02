// ReSharper disable once CheckNamespace
namespace Modules.AudioModule.Shared
{
    /// <summary>
    /// The sounds EconomyModule plays. Compiled into Modules.Audio.Shared by the asmref beside it;
    /// every id starts with "EconomyModule/", the bank at EconomyModule/Resources/Audio/CD_AudioBank.asset.
    /// </summary>
    public readonly partial struct AudioKey
    {
        public static class Economy
        {
            public static readonly AudioKey Buy = new("EconomyModule/Buy");
        }
    }
}
