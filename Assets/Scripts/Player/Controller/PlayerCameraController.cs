using FireLine.Scripts.Player.Model;
using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerCameraController : NetworkBehaviour
    {
        [Header("Camera")]
        [SerializeField]
        private Camera playerCamera;

        [Header("Follow")]
        [SerializeField]
        private Vector3 cameraOffset =
            new Vector3(0f, 10f, 0f);

        [SerializeField]
        private float followSpeed = 10f;

        private CameraShakeData _activeShakeData;

        private float _shakeTimer;
        private float _shakeSeed;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner)
                return;

            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }

            if (playerCamera == null)
            {
                Debug.LogError(
                    "[PLAYER-CAMERA] " +
                    "Main Camera not found!"
                );

                return;
            }

            playerCamera.enabled = true;

            Debug.Log(
                "[PLAYER-CAMERA] " +
                "Local camera assigned."
            );
        }

        private void LateUpdate()
        {
            if (!IsOwner)
                return;

            if (playerCamera == null)
                return;

            UpdateShake();

            Vector3 targetPosition =
                transform.position +
                cameraOffset;

            Vector3 followPosition =
                Vector3.Lerp(
                    playerCamera.transform.position,
                    targetPosition,
                    followSpeed * Time.deltaTime
                );

            playerCamera.transform.position =
                followPosition +
                GetShakeOffset();

            playerCamera.transform.rotation =
                Quaternion.Euler(
                    90f,
                    0f,
                    0f
                );
        }

        public void Shake(
            CameraShakeData shakeData)
        {
            if (!IsOwner)
                return;

            if (shakeData == null)
            {
                Debug.LogWarning(
                    "[PLAYER-CAMERA] " +
                    "CameraShakeData is NULL!"
                );

                return;
            }

            _activeShakeData =
                shakeData;

            _shakeTimer =
                shakeData.Duration;

            _shakeSeed =
                Random.Range(
                    0f,
                    1000f
                );
        }

        private void UpdateShake()
        {
            if (_shakeTimer <= 0f)
                return;

            _shakeTimer -=
                Time.deltaTime;

            if (_shakeTimer <= 0f)
            {
                _shakeTimer = 0f;
                _activeShakeData = null;
            }
        }

        private Vector3 GetShakeOffset()
        {
            if (_activeShakeData == null ||
                _shakeTimer <= 0f)
            {
                return Vector3.zero;
            }

            float normalizedTime =
                Mathf.Clamp01(
                    _shakeTimer /
                    _activeShakeData.Duration
                );

            float strength =
                _activeShakeData.Strength *
                normalizedTime;

            float time =
                (Time.time + _shakeSeed) *
                _activeShakeData.Frequency;

            float x =
                Mathf.PerlinNoise(
                    time,
                    0f
                ) - 0.5f;

            float z =
                Mathf.PerlinNoise(
                    0f,
                    time
                ) - 0.5f;

            return new Vector3(
                x,
                0f,
                z
            ) * strength;
        }
    }
}