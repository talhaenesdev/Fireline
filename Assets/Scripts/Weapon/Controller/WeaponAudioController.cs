using UnityEngine;

namespace FireLine.Scripts.Weapon.Controller
{
    public class WeaponAudioController : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] fireClips;

        public void PlayFireSound()
        {
            if (audioSource == null)
            {
                Debug.LogError(
                    "[WEAPON-AUDIO] AudioSource is NULL!"
                );

                return;
            }

            if (fireClips == null ||
                fireClips.Length == 0)
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

            audioSource.PlayOneShot(clip);
        }
    }
}