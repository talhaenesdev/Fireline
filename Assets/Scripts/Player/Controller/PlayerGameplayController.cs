using System;
using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerGameplayController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInputController inputController;

        [SerializeField]
        private PlayerAimController aimController;

        [SerializeField]
        private PlayerWeaponController weaponController;

        public event Action<Vector3> OnFire;

        public Vector3 MuzzlePosition =>
            weaponController != null
                ? weaponController.MuzzlePosition
                : transform.position;

        private void Update()
        {
            if (inputController == null ||
                aimController == null ||
                weaponController == null)
            {
                return;
            }

            if (inputController.FirePressed)
            {
                OnFire?.Invoke(
                    aimController.AimDirection
                );
            }
        }

        public void Shoot(Vector3 direction)
        {
            if (weaponController == null)
                return;

            weaponController.Shoot(direction);
        }
    }
}