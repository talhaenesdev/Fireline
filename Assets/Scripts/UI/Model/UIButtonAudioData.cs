using System.Collections.Generic;
using UnityEngine;

namespace FireLine.Scripts.UI.Model
{
    [CreateAssetMenu(
        fileName = "UIButtonAudioData",
        menuName = "FireLine/UI/Button Audio Data"
    )]
    public class UIButtonAudioData : ScriptableObject
    {
        [SerializeField]
        private List<UIButtonAudioEntry> entries =
            new List<UIButtonAudioEntry>();

        private Dictionary<UIButtonType, UIButtonAudioEntry>
            _entryDictionary;

        private void OnEnable()
        {
            BuildDictionary();
        }

        private void BuildDictionary()
        {
            _entryDictionary =
                new Dictionary<
                    UIButtonType,
                    UIButtonAudioEntry
                >();

            if (entries == null)
                return;

            foreach (UIButtonAudioEntry entry in entries)
            {
                if (entry == null)
                    continue;

                if (_entryDictionary.ContainsKey(entry.Type))
                {
                    Debug.LogWarning(
                        $"[UI AUDIO DATA] " +
                        $"Duplicate button type: {entry.Type} | " +
                        $"Asset={name}"
                    );

                    continue;
                }

                _entryDictionary.Add(
                    entry.Type,
                    entry
                );
            }
        }

        public AudioClip GetHoverClip(
            UIButtonType type)
        {
            EnsureDictionary();

            if (!_entryDictionary.TryGetValue(
                    type,
                    out UIButtonAudioEntry entry))
            {
                return null;
            }

            return entry.HoverClip;
        }

        public AudioClip GetClickClip(
            UIButtonType type)
        {
            EnsureDictionary();

            if (!_entryDictionary.TryGetValue(
                    type,
                    out UIButtonAudioEntry entry))
            {
                return null;
            }

            return entry.ClickClip;
        }

        private void EnsureDictionary()
        {
            if (_entryDictionary == null)
            {
                BuildDictionary();
            }
        }
    }
}