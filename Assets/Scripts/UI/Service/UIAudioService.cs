using FireLine.Scripts.UI.Model;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.UI.Service
{
    public class UIAudioService :
        IUIAudioService,
        IInitializable
    {
        private readonly UIButtonAudioData _audioData;

        private AudioSource _audioSource;

        private float _volume = 1f;

        public UIAudioService(
            UIButtonAudioData audioData)
        {
            _audioData = audioData;

            Debug.Log(
                "[UI AUDIO] " +
                "Constructor called."
            );
        }

        public void Initialize()
        {
            Debug.Log(
                "[UI AUDIO] " +
                "Initialize called."
            );

            CreateAudioSource();

            Debug.Log(
                $"[UI AUDIO] " +
                $"Initialize finished | " +
                $"AudioSource={_audioSource}"
            );
        }

        private void CreateAudioSource()
        {
            Debug.Log(
                "[UI AUDIO] " +
                "Creating AudioSource..."
            );

            GameObject audioObject =
                new GameObject(
                    "UIAudioService"
                );

            Object.DontDestroyOnLoad(
                audioObject
            );

            _audioSource =
                audioObject.AddComponent<
                    AudioSource>();

            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.spatialBlend = 0f;
            _audioSource.volume = _volume;

            Debug.Log(
                $"[UI AUDIO] " +
                $"AudioSource created | " +
                $"Object={audioObject.name} | " +
                $"Source={_audioSource}"
            );
        }

        public void PlayHover(
            UIButtonType type)
        {
            AudioClip clip =
                _audioData.GetHoverClip(type);

            PlayClip(clip);
        }

        public void PlayClick(
            UIButtonType type)
        {
            AudioClip clip =
                _audioData.GetClickClip(type);

            PlayClip(clip);
        }

        private void PlayClip(
            AudioClip clip)
        {
            Debug.Log(
                $"[UI AUDIO] PlayClip | " +
                $"Clip={clip} | " +
                $"AudioSource={_audioSource}"
            );

            if (_audioSource == null)
            {
                Debug.LogWarning(
                    "[UI AUDIO] " +
                    "AudioSource is NULL!"
                );

                return;
            }

            if (clip == null)
            {
                Debug.LogWarning(
                    "[UI AUDIO] " +
                    "AudioClip is NULL!"
                );

                return;
            }

            _audioSource.PlayOneShot(
                clip,
                _volume
            );
        }
    }
}