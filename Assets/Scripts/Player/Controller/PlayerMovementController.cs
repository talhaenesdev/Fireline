using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField]
        private float sprintSpeed = 8f;

        private PlayerInputController _inputController;
        private PlayerAnimationController _animationController;

        private void Awake()
        {
            _inputController =
                GetComponent<PlayerInputController>();

            _animationController =
                GetComponent<PlayerAnimationController>();
        }

        private void Update()
        {
            if (_inputController == null)
            {
                Debug.LogError(
                    "[MOVEMENT] InputController NULL!"
                );

                return;
            }

            Vector2 input =
                _inputController.MoveInput;

            _animationController?.SetMovement(
                input,
                _inputController.SprintInput
            );

            Vector3 movement =
                new Vector3(
                    input.x,
                    0f,
                    input.y
                );

            float currentSpeed =
                _inputController.SprintInput
                    ? sprintSpeed
                    : moveSpeed;

            transform.position +=
                movement *
                currentSpeed *
                Time.deltaTime;
        }
    }
}