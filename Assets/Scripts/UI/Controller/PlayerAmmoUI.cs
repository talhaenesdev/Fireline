using FireLine.Scripts.Player.Controller;
using TMPro;
using UnityEngine;

namespace FireLine.Scripts.UI.Controller
{
    public class PlayerAmmoUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField]
        private TMP_Text ammoText;

        private PlayerWeaponController _weapon;

        private void Update()
        {
            if (_weapon != null)
                return;

            TryFindLocalPlayer();
        }

        private void TryFindLocalPlayer()
        {
            PlayerWeaponController[] players =
                FindObjectsByType<PlayerWeaponController>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None
                );

            foreach (
                PlayerWeaponController player
                in players)
            {
                if (player == null)
                    continue;

                Unity.Netcode.NetworkObject networkObject =
                    player.GetComponent<Unity.Netcode.NetworkObject>();

                if (networkObject == null)
                    continue;

                if (!networkObject.IsOwner)
                    continue;

                Initialize(player);
                break;
            }
        }

        private void Initialize(
            PlayerWeaponController weapon)
        {
            if (_weapon != null)
            {
                _weapon.AmmoChanged -=
                    OnAmmoChanged;
            }

            _weapon = weapon;

            if (_weapon == null)
            {
                Debug.LogError(
                    "[AMMO UI] " +
                    "PlayerWeaponController is NULL!"
                );

                return;
            }

            _weapon.AmmoChanged +=
                OnAmmoChanged;

            UpdateUI(
                _weapon.CurrentAmmo,
                _weapon.MagazineSize
            );

            Debug.Log(
                $"[AMMO UI] Initialized | " +
                $"Ammo={_weapon.CurrentAmmo}/" +
                $"{_weapon.MagazineSize}"
            );
        }

        private void OnAmmoChanged(
            int currentAmmo,
            int magazineSize)
        {
            UpdateUI(
                currentAmmo,
                magazineSize
            );
        }

        private void UpdateUI(
            int currentAmmo,
            int magazineSize)
        {
            if (ammoText == null)
                return;

            ammoText.text =
                $"{currentAmmo} / {magazineSize}";
        }

        private void OnDestroy()
        {
            if (_weapon != null)
            {
                _weapon.AmmoChanged -=
                    OnAmmoChanged;
            }
        }
    }
}