// ReSharper disable once CheckNamespace
namespace Modules.AudioModule.Shared
{
    /// <summary>
    /// The sounds UiWidgetsModule plays. Compiled into Modules.Audio.Shared by the asmref beside it;
    /// every id starts with "UiWidgetsModule/", the bank at UiWidgetsModule/Resources/Audio/CD_AudioBank.asset.
    /// </summary>
    public readonly partial struct AudioKey
    {
        public static class UiWidgets
        {
            public static readonly AudioKey ButtonHover = new("UiWidgetsModule/ButtonHover");
            public static readonly AudioKey ButtonClick = new("UiWidgetsModule/ButtonClick");
        }
    }
}
