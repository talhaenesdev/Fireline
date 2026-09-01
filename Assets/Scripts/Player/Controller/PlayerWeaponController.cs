using FireLine.Scripts.Core.Weapon;
using FireLine.Scripts.Weapon.Controller;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerWeaponController : MonoBehaviour
    {
        private WeaponController _weaponController;
        private IWeaponFireService _fireService;
        private NetworkObject _networkObject;

        [SerializeField]
        private Transform muzzlePoint;

        // ============================================================
        // ZENJECT
        // ============================================================

        [Inject]
        public void Construct(
            WeaponController weaponController)
        {
            _weaponController = weaponController;

            Debug.Log(
                $"[PLAYER-WEAPON][INJECT] " +
                $"WeaponController injected | " +
                $"Instance={GetInstanceID()} | " +
                $"Scene={gameObject.scene.name}"
            );
        }

        // ============================================================
        // AWAKE
        // ============================================================

        private void Awake()
        {
            _networkObject =
                GetComponent<NetworkObject>();

            _fireService =
                GetComponent<IWeaponFireService>();

            Debug.Log(
                $"[PLAYER-WEAPON][AWAKE] " +
                $"Instance={GetInstanceID()} | " +
                $"GameObject={gameObject.name} | " +
                $"Scene={gameObject.scene.name} | " +
                $"NetworkObject={_networkObject != null} | " +
                $"FireService={_fireService != null}"
            );
        }

        // ============================================================
        // SHOOT
        // ============================================================

        public void Shoot(Vector3 direction)
        {
            Debug.Log(
                $"[PLAYER-WEAPON][SHOOT] " +
                $"Instance={GetInstanceID()} | " +
                $"GameObject={gameObject.name} | " +
                $"Owner={GetOwnerId()} | " +
                $"IsOwner={GetIsOwner()} | " +
                $"Controller={_weaponController != null}"
            );

            if (_weaponController == null)
            {
                Debug.LogError(
                    "[PLAYER-WEAPON][ERROR] " +
                    "WeaponController is NULL!"
                );

                return;
            }

            if (_fireService == null)
            {
                Debug.LogError(
                    "[PLAYER-WEAPON][ERROR] " +
                    "IWeaponFireService is NULL!"
                );

                return;
            }

            if (muzzlePoint == null)
            {
                Debug.LogError(
                    "[PLAYER-WEAPON][ERROR] " +
                    "MuzzlePoint is NULL!"
                );

                return;
            }

            if (direction == Vector3.zero)
            {
                Debug.LogWarning(
                    "[PLAYER-WEAPON][SHOOT] " +
                    "Shoot cancelled | Direction is zero"
                );

                return;
            }

            if (!_weaponController.CanShoot())
            {
                Debug.Log(
                    "[PLAYER-WEAPON][SHOOT] " +
                    "Shoot blocked | WeaponController.CanShoot=false"
                );

                return;
            }

            Vector3 position =
                muzzlePoint.position;

            _weaponController.RegisterShot();

            Debug.Log(
                $"[PLAYER-WEAPON][FIRE] " +
                $"Position={position} | " +
                $"Direction={direction}"
            );

            _fireService.Fire(
                position,
                direction
            );
        }

        // ============================================================
        // NETWORK HELPERS
        // ============================================================

        private ulong GetOwnerId()
        {
            return _networkObject != null
                ? _networkObject.OwnerClientId
                : ulong.MaxValue;
        }

        private bool GetIsOwner()
        {
            return _networkObject != null &&
                   _networkObject.IsOwner;
        }
    }
}