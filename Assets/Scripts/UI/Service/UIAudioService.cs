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
        }

        public void Initialize()
        {
            CreateAudioSource();

            Debug.Log(
                "[UI AUDIO] " +
                "UIAudioService initialized."
            );
        }

        private void CreateAudioSource()
        {
            GameObject audioObject =
                new GameObject("UIAudioService");

            Object.DontDestroyOnLoad(
                audioObject
            );

            _audioSource =
                audioObject.AddComponent<AudioSource>();

            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.spatialBlend = 0f;
            _audioSource.volume = _volume;
        }

        public void PlayHover(
            UIButtonType type)
        {
            if (_audioData == null)
            {
                Debug.LogWarning(
                    "[UI AUDIO] " +
                    "AudioData is NULL!"
                );

                return;
            }

            AudioClip clip =
                _audioData.GetHoverClip(type);

            PlayClip(clip);
        }

        public void PlayClick(
            UIButtonType type)
        {
            if (_audioData == null)
            {
                Debug.LogWarning(
                    "[UI AUDIO] " +
                    "AudioData is NULL!"
                );

                return;
            }

            AudioClip clip =
                _audioData.GetClickClip(type);

            PlayClip(clip);
        }

        private void PlayClip(
            AudioClip clip)
        {
            if (_audioSource == null)
            {
                Debug.LogWarning(
                    "[UI AUDIO] " +
                    "AudioSource is NULL!"
                );

                return;
            }

            if (clip == null)
                return;

            _audioSource.PlayOneShot(
                clip,
                _volume
            );
        }
    }
}