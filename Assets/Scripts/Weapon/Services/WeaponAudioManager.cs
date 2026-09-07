using UnityEngine;

namespace FireLine.Scripts.Weapon.Service
{
    public class WeaponAudioManager : MonoBehaviour
    {
        public static WeaponAudioManager Instance { get; private set; }

        [Header("Audio")]
        [SerializeField] private AudioClip[] fireClips;

        [Header("3D Settings")]
        [SerializeField] private float volume = 1f;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 15f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Debug.Log("[WEAPON-AUDIO] Manager initialized!");
        }

        public void PlayFireSound(Vector3 position)
        {
            Debug.Log(
                $"[WEAPON-AUDIO] PlayFireSound | Position={position}"
            );

            if (fireClips == null || fireClips.Length == 0)
            {
                Debug.LogWarning(
                    "[WEAPON-AUDIO] No fire clips assigned!"
                );

                return;
            }

            AudioClip clip =
                fireClips[
                    Random.Range(0, fireClips.Length)
                ];

            Debug.Log(
                $"[WEAPON-AUDIO] Clip={clip.name}"
            );

            GameObject audioObject =
                new GameObject("WeaponFireAudio");

            audioObject.transform.position = position;

            AudioSource audioSource =
                audioObject.AddComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.spatialBlend = 0f;
            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;
            audioSource.rolloffMode =
                AudioRolloffMode.Linear;

            audioSource.Play();

            Destroy(
                audioObject,
                clip.length
            );
        }
    }
}