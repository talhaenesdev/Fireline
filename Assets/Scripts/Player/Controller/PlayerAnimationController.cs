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

        [Header("Animation Layers")]
        [SerializeField]
        private int upperBodyLayerIndex = 1;

        private static readonly int MoveXHash =
            Animator.StringToHash("MoveX");

        private static readonly int MoveYHash =
            Animator.StringToHash("MoveY");

        private static readonly int SpeedHash =
            Animator.StringToHash("Speed");

        private static readonly int IsDeadHash =
            Animator.StringToHash("IsDead");


        private void Awake()
        {
            if (animator == null)
                animator =
                    GetComponentInChildren<Animator>();

            if (aimTransform == null)
                aimTransform = transform;

            if (animator == null)
            {
                Debug.LogError(
                    "[PLAYER ANIMATION] Animator not found!"
                );
            }
        }

        public void SetMovement(
    Vector2 input,
    bool isSprinting)
        {
            if (animator == null)
                return;

            if (input.sqrMagnitude < 0.001f)
            {
                animator.SetFloat(
                    MoveXHash,
                    0f
                );

                animator.SetFloat(
                    MoveYHash,
                    0f
                );

                animator.SetFloat(
                    SpeedHash,
                    0f
                );

                return;
            }

            Vector3 worldMovement =
                new Vector3(
                    input.x,
                    0f,
                    input.y
                );

            Vector3 localMovement =
                aimTransform.InverseTransformDirection(
                    worldMovement
                );

            animator.SetFloat(
                MoveXHash,
                localMovement.x
            );

            animator.SetFloat(
                MoveYHash,
                localMovement.z
            );

            float animationSpeed =
                isSprinting
                    ? 2f
                    : 1f;

            animator.SetFloat(
                SpeedHash,
                animationSpeed
            );
        }

        public void SetDeath(bool isDead)
        {
            if (animator == null)
                return;

            animator.SetBool(
                IsDeadHash,
                isDead
            );

            if (upperBodyLayerIndex >= 0 &&
                upperBodyLayerIndex < animator.layerCount)
            {
                animator.SetLayerWeight(
                    upperBodyLayerIndex,
                    isDead ? 0f : 1f
                );
            }

            Debug.Log(
                $"[PLAYER ANIMATION] " +
                $"Death={isDead} | " +
                $"UpperBodyWeight=" +
                $"{animator.GetLayerWeight(upperBodyLayerIndex)}"
            );
        }
    }
}