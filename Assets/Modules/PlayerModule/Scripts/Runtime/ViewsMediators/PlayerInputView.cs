using System;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Modules.PlayerModule.ViewsMediators
{
    /// <summary>The player's raw input, read from the PlayerInput beside it. Owner only.</summary>
    [RequireComponent(typeof(ViewInjector))]
    public class PlayerInputView : MonoBehaviour, IView
    {
        [SerializeField] private PlayerInput _playerInput;

        [Header("Actions")]
        [SerializeField] private string _moveAction = "Move";
        [SerializeField] private string _sprintAction = "Sprint";
        [SerializeField] private string _fireAction = "Attack";
        [SerializeField] private string _reloadAction = "Reload";

        public bool IsRegistered { get; set; }

        public Action<Vector2> MoveChanged;
        public Action<bool> SprintChanged;
        public Action<bool, bool> FireChanged;
        public Action ReloadPressed;

        private InputAction _move;
        private InputAction _sprint;
        private InputAction _fire;
        private InputAction _reload;

        // Read once a frame: asking from inside an Input System callback answers for the wrong frame.
        private bool _isPointerOverUi;

        public void Enable(bool isEnabled)
        {
            if (_playerInput == null)
                _playerInput = GetComponent<PlayerInput>();

            if (_playerInput == null)
                return;

            _playerInput.enabled = isEnabled;

            if (isEnabled)
                Hook();
            else
                Unhook();
        }

        private void OnDestroy() => Unhook();

        private void Update() =>
            _isPointerOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        private void Hook()
        {
            if (_move != null)
                return;

            _move = _playerInput.actions.FindAction(_moveAction, true);
            _sprint = _playerInput.actions.FindAction(_sprintAction, true);
            _fire = _playerInput.actions.FindAction(_fireAction, true);
            _reload = _playerInput.actions.FindAction(_reloadAction, true);

            _move.performed += OnMove;
            _move.canceled += OnMove;
            _sprint.performed += OnSprint;
            _sprint.canceled += OnSprint;
            _fire.started += OnFireStarted;
            _fire.canceled += OnFireCanceled;
            _reload.started += OnReload;
        }

        private void Unhook()
        {
            if (_move == null)
                return;

            _move.performed -= OnMove;
            _move.canceled -= OnMove;
            _sprint.performed -= OnSprint;
            _sprint.canceled -= OnSprint;
            _fire.started -= OnFireStarted;
            _fire.canceled -= OnFireCanceled;
            _reload.started -= OnReload;
            _move = _sprint = _fire = _reload = null;
        }

        private void OnMove(InputAction.CallbackContext context) => MoveChanged?.Invoke(context.ReadValue<Vector2>());
        private void OnSprint(InputAction.CallbackContext context) => SprintChanged?.Invoke(context.ReadValueAsButton());
        // A click on a button is the button's, not a shot - the exit button sits where the player aims.
        private void OnFireStarted(InputAction.CallbackContext context)
        {
            // A phone fires from its on-screen button; a tap anywhere else is not a shot.
            if (!_isPointerOverUi && !Application.isMobilePlatform)
                FireChanged?.Invoke(true, true);
        }

        private void OnFireCanceled(InputAction.CallbackContext context) => FireChanged?.Invoke(false, false);
        private void OnReload(InputAction.CallbackContext context) => ReloadPressed?.Invoke();
    }
}
