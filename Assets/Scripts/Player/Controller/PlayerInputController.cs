using UnityEngine;
using UnityEngine.InputSystem;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerInputController : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }

        public bool FirePressed { get; private set; }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();

            Debug.Log($"Move Input: {MoveInput}");
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            FirePressed = context.ReadValueAsButton();
        }
    }
}