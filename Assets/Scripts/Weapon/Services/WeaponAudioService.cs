using UnityEngine;

namespace FireLine.Scripts.Weapon.Service
{
    public class WeaponAudioService
    {
        private readonly AudioClip[] _fireClips;

        public WeaponAudioService(
            AudioClip[] fireClips)
        {
            _fireClips = fireClips;
        }

        public void PlayFireSound(
            Vector3 position)
        {
            if (_fireClips == null ||
                _fireClips.Length == 0)
            {
                Debug.LogWarning(
                    "[WEAPON-AUDIO] " +
                    "No fire clips assigned!"
                );

                return;
            }

            AudioClip clip =
                _fireClips[
                    Random.Range(0, _fireClips.Length)
                ];

            AudioSource.PlayClipAtPoint(
                clip,
                position
            );
        }
    }
}