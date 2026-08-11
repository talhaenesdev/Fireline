using FireLine.Scripts.Player.View;
using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerView view;

        private PlayerModel model;
        private Rigidbody rb;
        private PlayerInputActions inputActions;

        private Vector2 moveInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            model = new PlayerModel(
                moveSpeed: 5f,
                maxHealth: 100
            );

            inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();

            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnMove;
        }

        private void OnDisable()
        {
            inputActions.Player.Move.performed -= OnMove;
            inputActions.Player.Move.canceled -= OnMove;

            inputActions.Player.Disable();
        }

        private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            Vector3 direction = new Vector3(
                moveInput.x,
                0f,
                moveInput.y
            );

            Vector3 velocity = direction * model.MoveSpeed;

            rb.linearVelocity = new Vector3(
                velocity.x,
                rb.linearVelocity.y,
                velocity.z
            );
        }
    }

}
