using FireLine.Scripts.Audio.Service;
using UnityEngine;

namespace FireLine.Scripts.Weapon.Service
{
    public class WeaponAudioManager : MonoBehaviour
    {
        public static WeaponAudioManager Instance { get; private set; }

        [Header("Audio")]
        [SerializeField]
        private AudioClip[] fireClips;

        [Header("3D Settings")]
        [SerializeField]
        private float volume = 1f;

        [SerializeField]
        private float minDistance = 5f;

        [SerializeField]
        private float maxDistance = 30f;

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
                "[WEAPON-AUDIO] Manager initialized!"
            );
        }

        public void PlayFireSound(
            Vector3 position)
        {
            if (fireClips == null ||
                fireClips.Length == 0)
            {
                Debug.LogWarning(
                    "[WEAPON-AUDIO] " +
                    "No fire clips assigned!"
                );

                return;
            }

            if (AudioPoolManager.Instance == null)
            {
                Debug.LogError(
                    "[WEAPON-AUDIO] " +
                    "AudioPoolManager is NULL!"
                );

                return;
            }

            AudioClip clip =
                fireClips[
                    Random.Range(
                        0,
                        fireClips.Length
                    )
                ];

            AudioPoolManager.Instance.Play(
                clip,
                position,
                volume,
                minDistance,
                maxDistance
            );

            Debug.Log(
                $"[WEAPON-AUDIO] Playing | " +
                $"Clip={clip.name} | " +
                $"Position={position}"
            );
        }
    }
}