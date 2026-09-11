using UnityEngine;
using UnityEngine.InputSystem;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerInputController : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }

        public bool FirePressed { get; private set; }

        public bool FireStarted { get; private set; }

        public bool ReloadPressed { get; private set; }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            FirePressed = context.ReadValueAsButton();

            if (context.started)
            {
                FireStarted = true;
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ReloadPressed = true;

                Debug.Log("[INPUT] Reload pressed!");
            }
        }

        private void LateUpdate()
        {
            FireStarted = false;
            ReloadPressed = false;
        }
    }
}