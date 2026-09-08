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
        private Coroutine _reloadCoroutine;

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

            // ============================================================
            // RELOAD
            // ============================================================

            if (_inputController.ReloadPressed)
            {
                Debug.Log(
                    "[GAMEPLAY] Reload input received!"
                );

                StartReload();
            }

            // ============================================================
            // FIRE
            // ============================================================

            bool shouldFire;

            if (_weaponController.IsAutomatic())
            {
                shouldFire =
                    _inputController.FirePressed;
            }
            else
            {
                shouldFire =
                    _inputController.FireStarted;
            }

            if (!shouldFire)
                return;

            Debug.Log(
                $"[GAMEPLAY] Fire | " +
                $"OwnerClientId: {_networkObject.OwnerClientId} | " +
                $"IsOwner: {_networkObject.IsOwner} | " +
                $"Automatic: {_weaponController.IsAutomatic()}"
            );

            _weaponController.Shoot(
                _aimController.AimDirection
            );

            OnFire?.Invoke();
        }

        private void StartReload()
        {
            if (_reloadCoroutine != null)
            {
                Debug.Log("[GAMEPLAY] Reload already running!");
                return;
            }

            if (_weaponController.IsReloading)
            {
                Debug.Log("[GAMEPLAY] Weapon is already reloading!");
                return;
            }

            if (!_weaponController.StartReload())
            {
                Debug.Log("[GAMEPLAY] Reload cannot start!");
                return;
            }

            Debug.Log(
                $"[GAMEPLAY] Reload started | " +
                $"Ammo={_weaponController.CurrentAmmo}/" +
                $"{_weaponController.MagazineSize}"
            );

            _reloadCoroutine =
                StartCoroutine(ReloadRoutine());
        }

        private System.Collections.IEnumerator ReloadRoutine()
        {
            Debug.Log(
                $"[GAMEPLAY] Reloading... | " +
                $"Duration={_weaponController.ReloadDuration}"
            );

            yield return new WaitForSeconds(
                _weaponController.ReloadDuration
            );

            _weaponController.CompleteReload();

            Debug.Log(
                $"[GAMEPLAY] Reload completed | " +
                $"Ammo={_weaponController.CurrentAmmo}/" +
                $"{_weaponController.MagazineSize}"
            );

            _reloadCoroutine = null;
        }
    }
}