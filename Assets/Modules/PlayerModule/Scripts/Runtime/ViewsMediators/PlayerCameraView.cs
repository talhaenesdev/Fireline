using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using Modules.PlayerModule.Data.UnityObjects;
using UnityEngine;

namespace Modules.PlayerModule.ViewsMediators
{
    /// <summary>
    /// The top-down camera that follows the local avatar and shakes when it fires. How high it sits,
    /// how quickly it follows and how hard it shakes are the player's settings, applied through Apply.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class PlayerCameraView : MonoBehaviour, IView
    {
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Vector3 _cameraOffset = new(0f, 10f, 0f);
        [SerializeField] private float _followSpeed = 10f;
        [SerializeField] private CD_CameraShake _shake;
        [SerializeField] private CD_CameraSettings _settings;

        private float _shakeScale = 1f;

        private float _shakeTimer;
        private Vector3 _followPosition;
        private bool _hasFollowPosition;
        private float _shakeSeed;

        public bool IsRegistered { get; set; }

        public CD_CameraSettings Settings => _settings;

        private void Awake() => enabled = false;

        public void Follow()
        {
            if (_playerCamera == null)
                _playerCamera = Camera.main;

            if (_playerCamera == null)
                return;

            _playerCamera.enabled = true;
            enabled = true;
        }

        /// <summary>
        /// A top-down orthographic camera shows more of the arena the larger its size, so height is
        /// its size; a perspective camera is moved up instead.
        /// </summary>
        public void Apply(float height, float followSpeed, float shake)
        {
            _followSpeed = followSpeed;
            _shakeScale = shake;

            if (_playerCamera != null && _playerCamera.orthographic)
                _playerCamera.orthographicSize = height;
            else
                _cameraOffset.y = height;
        }

        public void Shake()
        {
            if (!enabled || _shake == null)
                return;

            _shakeTimer = _shake.Duration;
            _shakeSeed = Random.Range(0f, 1000f);
        }

        private void LateUpdate()
        {
            if (_playerCamera == null)
                return;

            Transform cameraTransform = _playerCamera.transform;
            Vector3 target = transform.position + _cameraOffset;

            // The follow keeps its own position, so a shake sits on top of it and is gone when it ends;
            // following from the shaken camera let every shot nudge the view and read as a push.
            if (!_hasFollowPosition)
            {
                _followPosition = target;
                _hasFollowPosition = true;
            }

            _followPosition = Vector3.Lerp(_followPosition, target, _followSpeed * Time.deltaTime);
            cameraTransform.position = _followPosition + ShakeOffset();
            cameraTransform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        private Vector3 ShakeOffset()
        {
            if (_shakeTimer <= 0f || _shake == null)
                return Vector3.zero;

            _shakeTimer = Mathf.Max(0f, _shakeTimer - Time.deltaTime);

            float strength = _shake.Strength * _shakeScale * Mathf.Clamp01(_shakeTimer / _shake.Duration);
            float time = (Time.time + _shakeSeed) * _shake.Frequency;
            float x = Mathf.PerlinNoise(time, 0f) - 0.5f;
            float z = Mathf.PerlinNoise(0f, time) - 0.5f;

            return new Vector3(x, 0f, z) * strength;
        }
    }
}
