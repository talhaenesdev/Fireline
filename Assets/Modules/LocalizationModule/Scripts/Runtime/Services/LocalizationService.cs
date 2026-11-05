using System;
using System.Collections.Generic;
using System.Text;
using FlowIoC.ConsoleModule;
using Modules.LocalizationModule.Enums;
using UnityEngine;

namespace Modules.LocalizationModule.Services
{
    internal class LocalizationService : ILocalizationService
    {
        private const string LANGUAGE_KEY = "LANGUAGE";
        private const string TABLES = "Localization";

        // Rows by source line; each row holds one cell per language, in Language order.
        private readonly Dictionary<string, string[]> _rows = new();
#if UNITY_EDITOR
        private readonly HashSet<string> _reportedMissing = new();
#endif
        private bool _isLoaded;
        private Language _current;
        private bool _hasLanguage;

        public event Action LanguageChanged;

        public Language Current
        {
            get
            {
                if (!_hasLanguage)
                {
                    _current = ReadLanguage();
                    _hasLanguage = true;
                }

                return _current;
            }
        }

        public string Get(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            Load();
            if (!_rows.TryGetValue(text, out string[] row))
            {
#if UNITY_EDITOR
                if (_reportedMissing.Add(text))
                    FlowLogger.LogWarning($"Localization - no row for \"{text}\". Add it to a module's Resources/Localization table.");
#endif
                return text;
            }

            string cell = row[(int) Current];
            return string.IsNullOrEmpty(cell) ? text : cell;
        }

        public string Format(string text, params object[] args)
        {
            string line = Get(text);
            try
            {
                return string.Format(line, args);
            }
            catch (FormatException)
            {
                FlowLogger.LogError($"Localization - the line \"{line}\" for \"{text}\" does not take {args.Length} values.");
                return string.Format(text, args);
            }
        }

        public void SetLanguage(Language language)
        {
            if (_hasLanguage && _current == language)
                return;

            _current = language;
            _hasLanguage = true;
            PlayerPrefs.SetInt(LANGUAGE_KEY, (int) language);
            PlayerPrefs.Save();
            LanguageChanged?.Invoke();
        }

        public void SetNextLanguage()
        {
            int count = Enum.GetValues(typeof(Language)).Length;
            SetLanguage((Language) (((int) Current + 1) % count));
        }

        private static Language ReadLanguage()
        {
            if (PlayerPrefs.HasKey(LANGUAGE_KEY))
            {
                int stored = PlayerPrefs.GetInt(LANGUAGE_KEY);
                if (Enum.IsDefined(typeof(Language), stored))
                    return (Language) stored;
            }

            return Application.systemLanguage == SystemLanguage.Turkish ? Language.Turkish : Language.English;
        }

        // Every module's table, read once. A table is "key,tr,en"; a later table's row for the same
        // key fills only the cells the earlier one left empty.
        private void Load()
        {
            if (_isLoaded)
                return;

            _isLoaded = true;
            int languages = Enum.GetValues(typeof(Language)).Length;
            foreach (TextAsset table in Resources.LoadAll<TextAsset>(TABLES))
            {
                List<List<string>> lines = ParseCsv(table.text);
                for (int i = 1; i < lines.Count; i++)
                {
                    List<string> cells = lines[i];
                    if (cells.Count == 0 || string.IsNullOrEmpty(cells[0]) || cells[0].StartsWith("#"))
                        continue;

                    // A written \n in a cell is a line break, in the key as in the translations.
                    string key = cells[0].Replace("\\n", "\n");
                    if (!_rows.TryGetValue(key, out string[] row))
                        _rows[key] = row = new string[languages];

                    for (int language = 0; language < languages && language + 1 < cells.Count; language++)
                    {
                        if (string.IsNullOrEmpty(row[language]))
                            row[language] = cells[language + 1].Replace("\\n", "\n");
                    }
                }
            }
        }

        // Comma separated, a cell in double quotes may hold commas, line breaks and "" for a quote.
        private static List<List<string>> ParseCsv(string text)
        {
            var lines = new List<List<string>>();
            var cells = new List<string>();
            var cell = new StringBuilder();
            bool isQuoted = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (isQuoted)
                {
                    if (c == '"' && i + 1 < text.Length && text[i + 1] == '"')
                    {
                        cell.Append('"');
                        i++;
                    }
                    else if (c == '"')
                        isQuoted = false;
                    else
                        cell.Append(c);
                    continue;
                }

                switch (c)
                {
                    case '"':
                        isQuoted = true;
                        break;
                    case ',':
                        cells.Add(cell.ToString());
                        cell.Clear();
                        break;
                    case '\r':
                        break;
                    case '\n':
                        cells.Add(cell.ToString());
                        cell.Clear();
                        lines.Add(cells);
                        cells = new List<string>();
                        break;
                    default:
                        cell.Append(c);
                        break;
                }
            }

            if (cell.Length > 0 || cells.Count > 0)
            {
                cells.Add(cell.ToString());
                lines.Add(cells);
            }

            return lines;
        }
    }
}
