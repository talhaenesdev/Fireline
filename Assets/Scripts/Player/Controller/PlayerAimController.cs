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

        private void Awake()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }

            if (aimTransform == null)
            {
                aimTransform = transform;
            }
        }

        private void Update()
        {
            if (playerCamera == null)
            {
                Debug.LogError("PlayerAimController: Camera is null.");
                return;
            }

            if (Mouse.current == null)
            {
                Debug.LogError("PlayerAimController: Mouse.current is null.");
                return;
            }

            Vector2 mousePosition =
                Mouse.current.position.ReadValue();

            Ray ray =
                playerCamera.ScreenPointToRay(mousePosition);

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
    }
}