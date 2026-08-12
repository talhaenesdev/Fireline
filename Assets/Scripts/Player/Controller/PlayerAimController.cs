using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerAimController : ITickable
    {
        private readonly PlayerController _playerController;
        private readonly Camera _camera;

        public PlayerAimController(
            PlayerController playerController,
            Camera camera)
        {
            _playerController = playerController;
            _camera = camera;
        }

        public void Tick()
        {
            if (Mouse.current == null)
                return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();

            Ray ray = _camera.ScreenPointToRay(mousePosition);

            Plane groundPlane = new Plane(
                Vector3.up,
                Vector3.zero
            );

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPosition = ray.GetPoint(distance);

                _playerController.Aim(worldPosition);
            }
        }
    }
}