using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerGameplayController : MonoBehaviour
    {
        private PlayerInputController _inputController;
        private PlayerAimController _aimController;
        private PlayerWeaponController _weaponController;
        private NetworkObject _networkObject;

        public event System.Action OnFire;

        private void Awake()
        {
            _inputController =
                GetComponent<PlayerInputController>();

            _aimController =
                GetComponent<PlayerAimController>();

            _weaponController =
                GetComponent<PlayerWeaponController>();

            _networkObject =
                GetComponent<NetworkObject>();

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

            if (_networkObject == null)
            {
                Debug.LogError(
                    "[GAMEPLAY] " +
                    "NetworkObject NOT FOUND!"
                );
            }
        }

        private void Update()
        {
            if (_networkObject == null)
            {
                Debug.LogWarning(
                    "[GAMEPLAY] NetworkObject is NULL!"
                );

                return;
            }

            if (!_networkObject.IsOwner)
                return;

            if (_inputController == null ||
                _aimController == null ||
                _weaponController == null)
            {
                return;
            }

            if (!_inputController.FirePressed)
                return;

            Debug.Log(
                $"[GAMEPLAY] FirePressed | " +
                $"OwnerClientId: {_networkObject.OwnerClientId} | " +
                $"IsOwner: {_networkObject.IsOwner}"
            );

            _weaponController.Shoot(
                _aimController.AimDirection
            );

            OnFire?.Invoke();
        }
    }
}