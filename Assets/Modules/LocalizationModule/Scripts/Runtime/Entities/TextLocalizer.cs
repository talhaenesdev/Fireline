using System;
using System.Collections.Generic;
using TMPro;

namespace Modules.LocalizationModule.Entities
{
    /// <summary>
    /// What a View translates with. A View is never injected, so its Mediator fills this from
    /// ILocalizationService; until then every line answers itself. A label bound here is rewritten
    /// whenever the localizer is refilled, and Changed tells the View to redraw what it composes.
    /// </summary>
    public class TextLocalizer
    {
        private readonly List<(TMP_Text label, string text)> _labels = new();
        private Func<string, string> _get = text => text;

        /// <summary>The localizer was refilled: the language may have changed.</summary>
        public event Action Changed;

        /// <summary>The current language's line for this source text.</summary>
        public string Get(string text) => string.IsNullOrEmpty(text) ? text : _get(text);

        /// <summary>The translated line with its placeholders filled.</summary>
        public string Format(string text, params object[] args)
        {
            try
            {
                return string.Format(Get(text), args);
            }
            catch (FormatException)
            {
                return string.Format(text, args);
            }
        }

        /// <summary>Shows this source text on the label now and after every change of language.</summary>
        public TMP_Text Bind(TMP_Text label, string text)
        {
            if (label == null)
                return null;

            for (int i = _labels.Count - 1; i >= 0; i--)
            {
                if (_labels[i].label == label || _labels[i].label == null)
                    _labels.RemoveAt(i);
            }

            _labels.Add((label, text));
            label.text = Get(text);
            return label;
        }

        /// <summary>Takes the lookup the service gives and rewrites every bound label with it.</summary>
        public void Use(Func<string, string> get)
        {
            _get = get ?? (text => text);
            foreach ((TMP_Text label, string text) in _labels)
            {
                if (label != null)
                    label.text = Get(text);
            }

            Changed?.Invoke();
        }
    }
}
