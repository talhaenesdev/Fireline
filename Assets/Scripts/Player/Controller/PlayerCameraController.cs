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
                    "[PLAYER-CAMERA] Main Camera not found!"
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

            Vector3 targetPosition =
                transform.position +
                cameraOffset;

            playerCamera.transform.position =
                Vector3.Lerp(
                    playerCamera.transform.position,
                    targetPosition,
                    followSpeed * Time.deltaTime
                );

            // Kameranın yönü SABİT.
            // Player'ın rotation'ından etkilenmez.
            playerCamera.transform.rotation =
                Quaternion.Euler(
                    90f,
                    0f,
                    0f
                );
        }
    }
}