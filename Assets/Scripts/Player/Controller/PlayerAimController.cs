using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerAimController : MonoBehaviour
    {
        [SerializeField]
        private Camera playerCamera;

        [SerializeField]
        private Transform aimTransform;

        public Vector3 AimDirection { get; private set; }

        private void Update()
        {
            if (playerCamera == null)
                return;

            Vector2 mousePosition =
                UnityEngine.InputSystem.Mouse.current.position.ReadValue();

            Ray ray =
                playerCamera.ScreenPointToRay(mousePosition);

            Plane groundPlane =
                new Plane(Vector3.up, transform.position);

            if (!groundPlane.Raycast(
                    ray,
                    out float distance))
            {
                return;
            }

            Vector3 target =
                ray.GetPoint(distance);

            Vector3 direction =
                target - aimTransform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            AimDirection =
                direction.normalized;

            aimTransform.rotation =
                Quaternion.LookRotation(
                    AimDirection
                );
        }
    }
}