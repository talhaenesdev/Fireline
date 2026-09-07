using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerFootstepAudioController : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] footstepClips;

        [Header("Footstep")]
        [SerializeField] private float stepInterval = 0.35f;
        [SerializeField] private float movementThreshold = 0.01f;

        private Vector3 _lastPosition;
        private float _stepTimer;

        private void Awake()
        {
            if (audioSource == null)
            {
                audioSource = GetComponentInChildren<AudioSource>();
            }

            _lastPosition = transform.position;
        }

        private void Update()
        {
            Vector3 currentPosition = transform.position;

            Vector3 movement = currentPosition - _lastPosition;
            movement.y = 0f;

            bool isMoving =
                movement.sqrMagnitude >
                movementThreshold * movementThreshold;

            if (isMoving)
            {
                HandleFootsteps();
            }
            else
            {
                _stepTimer = 0f;

                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }

            _lastPosition = currentPosition;
        }

        private void HandleFootsteps()
        {
            if (audioSource.isPlaying)
                return;

            _stepTimer += Time.deltaTime;

            if (_stepTimer < stepInterval)
                return;

            PlayFootstep();

            _stepTimer = 0f;
        }

        private void PlayFootstep()
        {
            if (audioSource == null)
                return;

            if (footstepClips == null ||
                footstepClips.Length == 0)
                return;

            AudioClip clip =
                footstepClips[
                    Random.Range(0, footstepClips.Length)
                ];

            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.Play();
        }
    }
}