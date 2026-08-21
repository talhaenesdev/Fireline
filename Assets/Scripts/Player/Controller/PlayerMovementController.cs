using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 5f;

        private PlayerInputController _inputController;

        private void Awake()
        {
            _inputController =
                GetComponent<PlayerInputController>();
        }

        private void Update()
        {
            if (_inputController == null)
                return;

            Vector2 input = _inputController.MoveInput;

            Vector3 movement = new Vector3(
                input.x,
                0f,
                input.y
            );

            transform.position +=
                movement * moveSpeed * Time.deltaTime;
        }
    }
}