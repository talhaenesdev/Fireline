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

                weaponController.Shoot(
                    aimController.AimDirection
                );
            }
        }
    }
}