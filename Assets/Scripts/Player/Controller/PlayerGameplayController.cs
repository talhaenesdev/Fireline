using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerGameplayController : MonoBehaviour
    {
        private PlayerInputController _inputController;
        private PlayerAimController _aimController;
        private PlayerWeaponController _weaponController;

        public event System.Action OnFire;

        private void Awake()
        {
            _inputController =
                GetComponent<PlayerInputController>();

            _aimController =
                GetComponent<PlayerAimController>();

            _weaponController =
                GetComponent<PlayerWeaponController>();

            if (_inputController == null)
            {
                Debug.LogError(
                    "[GAMEPLAY] " +
                    "PlayerInputController NOT FOUND!"
                );
            }

            if (_aimController == null)
            {
                Debug.LogError(
                    "[GAMEPLAY] " +
                    "PlayerAimController NOT FOUND!"
                );
            }

            if (_weaponController == null)
            {
                Debug.LogError(
                    "[GAMEPLAY] " +
                    "PlayerWeaponController NOT FOUND!"
                );
            }
        }

        private void Update()
        {
            if (_inputController == null ||
                _aimController == null ||
                _weaponController == null)
            {
                return;
            }

            if (!_inputController.FirePressed)
                return;

            Debug.Log(
                "[GAMEPLAY] FirePressed"
            );

            _weaponController.Shoot(
                _aimController.AimDirection
            );

            OnFire?.Invoke();
        }
    }
}