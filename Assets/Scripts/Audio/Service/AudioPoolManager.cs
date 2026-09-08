using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FireLine.Scripts.Audio.Service
{
    public class AudioPoolManager : MonoBehaviour
    {
        public static AudioPoolManager Instance { get; private set; }

        [Header("Pool")]
        [SerializeField]
        private int initialPoolSize = 20;

        [SerializeField]
        private int maxPoolSize = 50;

        [Header("3D Settings")]
        [SerializeField]
        private float defaultMinDistance = 0.1f;

        [SerializeField]
        private float defaultMaxDistance = 20f;

        private readonly List<AudioSource> _audioSources =
            new List<AudioSource>();

        private readonly HashSet<AudioSource> _activeSources =
            new HashSet<AudioSource>();

        private Transform _poolRoot;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            CreatePoolRoot();
            InitializePool();

            Debug.Log(
                $"[AUDIO POOL] Initialized | " +
                $"Size={_audioSources.Count}"
            );
        }

        private void CreatePoolRoot()
        {
            GameObject root =
                new GameObject("AudioPool");

            root.transform.SetParent(transform);

            _poolRoot = root.transform;
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateAudioSource();
            }
        }

        private AudioSource CreateAudioSource()
        {
            if (_audioSources.Count >= maxPoolSize)
                return null;

            GameObject audioObject =
                new GameObject(
                    $"PooledAudio_{_audioSources.Count}"
                );

            audioObject.transform.SetParent(_poolRoot);

            AudioSource audioSource =
                audioObject.AddComponent<AudioSource>();

            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance =
                defaultMinDistance;
            audioSource.maxDistance =
                defaultMaxDistance;
            audioSource.rolloffMode =
                AudioRolloffMode.Linear;

            _audioSources.Add(audioSource);

            return audioSource;
        }

        public void Play(
            AudioClip clip,
            Vector3 position,
            float volume = 1f,
            float minDistance = -1f,
            float maxDistance = -1f)
        {
            if (clip == null)
            {
                Debug.LogWarning(
                    "[AUDIO POOL] Clip is NULL!"
                );

                return;
            }

            AudioSource audioSource =
                GetAvailableAudioSource();

            if (audioSource == null)
            {
                Debug.LogWarning(
                    "[AUDIO POOL] " +
                    "No available AudioSource!"
                );

                return;
            }

            audioSource.transform.position =
                position;

            audioSource.clip = clip;
            audioSource.volume = volume;

            audioSource.minDistance =
                minDistance > 0f
                    ? minDistance
                    : defaultMinDistance;

            audioSource.maxDistance =
                maxDistance > 0f
                    ? maxDistance
                    : defaultMaxDistance;

            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode =
                AudioRolloffMode.Linear;

            _activeSources.Add(audioSource);

            audioSource.Play();

            Debug.Log(
              $"[AUDIO TEST] " +
              $"Clip={clip.name} | " +
              $"Playing={audioSource.isPlaying} | " +
              $"Volume={audioSource.volume} | " +
              $"Mute={audioSource.mute}"
          );

            StartCoroutine(
                ReleaseAfterPlay(
                    audioSource,
                    clip.length
                )
            );
        }

        private AudioSource GetAvailableAudioSource()
        {
            for (int i = 0;
                 i < _audioSources.Count;
                 i++)
            {
                AudioSource source =
                    _audioSources[i];

                if (!_activeSources.Contains(source))
                {
                    return source;
                }
            }

            return CreateAudioSource();
        }

        private IEnumerator ReleaseAfterPlay(
            AudioSource audioSource,
            float duration)
        {
            yield return new WaitForSeconds(duration);

            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = null;

                _activeSources.Remove(
                    audioSource
                );
            }
        }
    }
}