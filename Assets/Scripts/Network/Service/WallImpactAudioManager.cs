using FireLine.Scripts.Audio.Service;
using UnityEngine;

namespace FireLine.Scripts.Network.Service
{
    public class WallImpactAudioManager : MonoBehaviour
    {
        public static WallImpactAudioManager Instance { get; private set; }

        [Header("Audio")]
        [SerializeField]
        private AudioClip[] impactClips;

        [Header("3D Settings")]
        [SerializeField]
        private float volume = 1f;

        [SerializeField]
        private float minDistance = 2f;

        [SerializeField]
        private float maxDistance = 20f;

        private void Awake()
        {
            if (Instance != null &&
                Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Debug.Log(
                "[WALL IMPACT AUDIO] " +
                "Manager initialized!"
            );
        }

        public void PlayImpactSound(Vector3 position)
        {
            if (impactClips == null || impactClips.Length == 0)
            {
                Debug.LogWarning(
                    "[WALL IMPACT AUDIO] No impact clips assigned!"
                );

                return;
            }

            AudioClip clip =
                impactClips[
                    Random.Range(0, impactClips.Length)
                ];

            Debug.Log(
                $"[WALL IMPACT AUDIO] Playing | " +
                $"Clip={clip.name} | " +
                $"Position={position}"
            );

            if (AudioPoolManager.Instance == null)
            {
                Debug.LogError(
                    "[WALL IMPACT AUDIO] AudioPoolManager.Instance IS NULL!"
                );

                return;
            }

            Debug.Log(
                "[WALL IMPACT AUDIO] Sending to AudioPool"
            );

            AudioPoolManager.Instance.Play(
                clip,
                position,
                volume,
                minDistance,
                maxDistance
            );
        }
    }
}