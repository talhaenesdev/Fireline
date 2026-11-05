using System;
using Modules.LocalizationModule.Enums;

namespace Modules.LocalizationModule.Services
{
    /// <summary>
    /// Every text the player reads. A text is its own key: ask for the source line and the current
    /// language's translation comes back, or the source itself when no table has one.
    /// </summary>
    public partial interface ILocalizationService
    {
        /// <summary>The language texts are answered in.</summary>
        Language Current { get; }

        /// <summary>Raised once the language has changed, so whatever is on screen can be redrawn.</summary>
        event Action LanguageChanged;

        /// <summary>The current language's text for this source line, or the line itself.</summary>
        string Get(string text);

        /// <summary>The translated line with its placeholders filled, as string.Format fills them.</summary>
        string Format(string text, params object[] args);

        /// <summary>Switches to this language and keeps it for the next run.</summary>
        void SetLanguage(Language language);

        /// <summary>Switches to the language after the current one, around the list.</summary>
        void SetNextLanguage();

        public static partial class Commands
        {
        }
    }
}
