using UnityEngine;

namespace Modules.PlayerModule.ViewsMediators
{
    /// <summary>
    /// Plays a step from the avatar's own source while it moves, for every player on every screen.
    /// It reads only its own transform and decides nothing, so it has no Mediator.
    /// </summary>
    public class PlayerFootstepAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _footstepClips;
        [SerializeField] private float _stepInterval = 0.35f;
        [SerializeField] private float _movementThreshold = 0.01f;

        private Vector3 _lastPosition;
        private float _stepTimer;

        private void Awake()
        {
            if (_audioSource == null)
                _audioSource = GetComponentInChildren<AudioSource>();

            _lastPosition = transform.position;
        }

        private void Update()
        {
            Vector3 movement = transform.position - _lastPosition;
            movement.y = 0f;
            _lastPosition = transform.position;

            if (movement.sqrMagnitude > _movementThreshold * _movementThreshold)
            {
                Step();
                return;
            }

            _stepTimer = 0f;
            if (_audioSource != null && _audioSource.isPlaying)
                _audioSource.Stop();
        }

        private void Step()
        {
            if (_audioSource == null || _audioSource.isPlaying || _footstepClips == null || _footstepClips.Length == 0)
                return;

            _stepTimer += Time.deltaTime;
            if (_stepTimer < _stepInterval)
                return;

            _stepTimer = 0f;
            _audioSource.clip = _footstepClips[Random.Range(0, _footstepClips.Length)];
            _audioSource.loop = false;
            _audioSource.Play();
        }
    }
}
