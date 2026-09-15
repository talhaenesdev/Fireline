using System;
using UnityEngine;

namespace FireLine.Scripts.UI.Model
{
    [Serializable]
    public class UIButtonAudioEntry
    {
        public UIButtonType Type;

        [Header("Audio")]
        public AudioClip HoverClip;
        public AudioClip ClickClip;
    }
}