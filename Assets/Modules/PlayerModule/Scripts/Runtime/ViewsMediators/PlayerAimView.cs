using System;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Modules.PlayerModule.ViewsMediators
{
    /// <summary>
    /// Turns the mouse into a direction on the ground plane around the avatar, and faces it. The
    /// mouse is raw input, so reading it is this view's job; only the owner's view is enabled. A
    /// phone has no mouse: there the avatar is turned through Face.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class PlayerAimView : MonoBehaviour, IView
    {
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Transform _aimTransform;

        public bool IsRegistered { get; set; }

        public Action<Vector3> AimChanged;

        private void Awake()
        {
            if (_aimTransform == null)
                _aimTransform = transform;

            enabled = false;
        }

        public void Enable(bool isEnabled) => enabled = isEnabled;

        public void Face(Vector3 direction)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
                _aimTransform.forward = direction.normalized;
        }

        private void Update()
        {
            if (Application.isMobilePlatform)
                return;

            if (_playerCamera == null)
                _playerCamera = Camera.main;

            if (_playerCamera == null || Mouse.current == null)
                return;

            Ray ray = _playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            var ground = new Plane(Vector3.up, _aimTransform.position);

            if (!ground.Raycast(ray, out float distance))
                return;

            Vector3 direction = ray.GetPoint(distance) - _aimTransform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            direction.Normalize();
            _aimTransform.forward = direction;
            AimChanged?.Invoke(direction);
        }
    }
}
