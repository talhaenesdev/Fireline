using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Transform aimTransform;

        private static readonly int MoveXHash =
            Animator.StringToHash("MoveX");

        private static readonly int MoveYHash =
            Animator.StringToHash("MoveY");

        private static readonly int SpeedHash =
            Animator.StringToHash("Speed");

        private void Awake()
        {
            if (animator == null)
            {
                animator =
                    GetComponentInChildren<Animator>();
            }

            if (aimTransform == null)
            {
                aimTransform = transform;
            }

            if (animator == null)
            {
                Debug.LogError(
                    "[PLAYER ANIMATION] Animator not found!"
                );
            }
        }

        public void SetMovement(Vector2 input)
        {
            if (animator == null)
                return;

            Vector3 worldMovement =
                new Vector3(
                    input.x,
                    0f,
                    input.y
                );

            float speed =
                worldMovement.magnitude;

            if (speed > 1f)
            {
                worldMovement.Normalize();
            }

            Vector3 localMovement =
                aimTransform.InverseTransformDirection(
                    worldMovement
                );

            float moveX = localMovement.x;
            float moveY = localMovement.z;

            animator.SetFloat(
                MoveXHash,
                moveX
            );

            animator.SetFloat(
                MoveYHash,
                moveY
            );

            animator.SetFloat(
                SpeedHash,
                speed
            );
        }
    }
}