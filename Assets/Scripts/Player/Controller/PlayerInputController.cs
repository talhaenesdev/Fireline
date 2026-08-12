using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerInputController : ITickable
    {
        private readonly PlayerController _playerController;

        public PlayerInputController(PlayerController playerController)
        {
            _playerController = playerController;
        }

        public void Tick()
        {
            Vector2 movement = Vector2.zero;

            if (Keyboard.current.wKey.isPressed)
                movement.y += 1f;

            if (Keyboard.current.sKey.isPressed)
                movement.y -= 1f;

            if (Keyboard.current.aKey.isPressed)
                movement.x -= 1f;

            if (Keyboard.current.dKey.isPressed)
                movement.x += 1f;

            movement = Vector2.ClampMagnitude(movement, 1f);

            if (movement != Vector2.zero)
            {
                _playerController.Move(movement);
            }
        }
    }
}