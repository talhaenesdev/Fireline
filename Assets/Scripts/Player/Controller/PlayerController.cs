using FireLine.Scripts.Player.View;
using FireLine.Scripts.Weapon.Controller;
using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private WeaponController weapon;
        [SerializeField] private PlayerView view;
        [SerializeField] private Camera playerCamera;

        private PlayerModel model;
        private Rigidbody rb;
        private PlayerInputActions inputActions;

        private Vector2 moveInput;
        private Vector2 aimInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            model = new PlayerModel(
                moveSpeed: 5f,
                maxHealth: 100
            );

            inputActions = new PlayerInputActions();

            if (playerCamera == null)
                playerCamera = Camera.main;
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();

            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnMove;

            inputActions.Player.Aim.performed += OnAim;
            inputActions.Player.Aim.canceled += OnAim;

            inputActions.Player.Fire.performed += OnFire;
        }

        private void OnDisable()
        {
            inputActions.Player.Move.performed -= OnMove;
            inputActions.Player.Move.canceled -= OnMove;

            inputActions.Player.Aim.performed -= OnAim;
            inputActions.Player.Aim.canceled -= OnAim;

            inputActions.Player.Disable();

            inputActions.Player.Fire.performed -= OnFire;
        }
        private void OnFire(
    UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            weapon.TryFire();
        }
        private void OnMove(
            UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void OnAim(
            UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            aimInput = context.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void Update()
        {
            HandleAim();
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

        private void HandleAim()
        {
            Ray ray = playerCamera.ScreenPointToRay(aimInput);

            Plane groundPlane = new Plane(
                Vector3.up,
                Vector3.zero
            );

            if (!groundPlane.Raycast(ray, out float distance))
                return;

            Vector3 targetPoint = ray.GetPoint(distance);

            Vector3 direction = targetPoint - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            view.SetRotation(targetRotation);
        }
    }

}
