using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerAimController : MonoBehaviour
    {
        [SerializeField]
        private Camera playerCamera;

        [SerializeField]
        private Transform aimTransform;

        public Vector3 AimDirection { get; private set; }
        private NetworkObject _networkObject;

        private void Awake()
        {
            if (aimTransform == null)
            {
                aimTransform = transform;
            }
            _networkObject =
                GetComponent<NetworkObject>();
            // Oyuncu oluşturulduğunda kamerayı bulmayı dene.
            TryFindCamera();
        }

        private void Update()
        {
            if (_networkObject == null ||
                !_networkObject.IsOwner)
            {
                return;
            }
            // Kamera henüz bulunamadıysa tekrar ara.
            if (playerCamera == null)
            {
                TryFindCamera();

                // Bu frame'de de kamera yoksa devam etme.
                if (playerCamera == null)
                {
                    return;
                }
            }

            if (Mouse.current == null)
            {
                Debug.LogError(
                    "PlayerAimController: " +
                    "Mouse.current is null."
                );

                return;
            }

            Vector2 mousePosition =
                Mouse.current.position.ReadValue();

            Ray ray =
                playerCamera.ScreenPointToRay(
                    mousePosition
                );

            Plane groundPlane =
                new Plane(
                    Vector3.up,
                    aimTransform.position
                );

            if (!groundPlane.Raycast(
                    ray,
                    out float distance))
            {
                return;
            }

            Vector3 mouseWorldPosition =
                ray.GetPoint(distance);

            Vector3 direction =
                mouseWorldPosition -
                aimTransform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            AimDirection =
                direction.normalized;

            aimTransform.forward =
                AimDirection;
        }

        private void TryFindCamera()
        {
            if (playerCamera != null)
                return;

            Camera[] cameras =
                FindObjectsByType<Camera>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None
                );

            foreach (Camera camera in cameras)
            {
                if (!camera.CompareTag("MainCamera"))
                    continue;

                playerCamera = camera;

                Debug.Log(
                    $"[PLAYER AIM] Camera assigned: " +
                    $"{camera.name}"
                );

                return;
            }

            Debug.LogWarning(
                "[PLAYER AIM] " +
                "Main Camera not found yet."
            );
        }
    }
}