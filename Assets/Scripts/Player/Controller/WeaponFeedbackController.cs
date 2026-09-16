using FireLine.Scripts.Player.Model;
using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class WeaponFeedbackController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private ParticleSystem muzzleFlash;

        [SerializeField]
        private Animator animator;

        [Header("Animation")]
        [SerializeField]
        private string fireTrigger = "Fire";

        [Header("Camera Shake")]
        [SerializeField]
        private CameraShakeData cameraShakeData;

        private PlayerWeaponController _weaponController;
        private PlayerCameraController _cameraController;
        private void Awake()
        {
            _weaponController =
                GetComponent<PlayerWeaponController>();
            _cameraController =
                GetComponent<PlayerCameraController>();
            if (_weaponController == null)
            {
                Debug.LogError(
                    "[WEAPON FEEDBACK] " +
                    "PlayerWeaponController not found!"
                );
            }

            if (animator == null)
            {
                animator =
                    GetComponentInChildren<Animator>();
            }
        }

        private void OnEnable()
        {
            if (_weaponController == null)
                return;

            _weaponController.Fired +=
                OnWeaponFired;
        }

        private void OnDisable()
        {
            if (_weaponController == null)
                return;

            _weaponController.Fired -=
                OnWeaponFired;
        }

        private void OnWeaponFired()
        {
            PlayMuzzleFlash();
            PlayFireAnimation();
            PlayCameraShake();
        }
        private void PlayCameraShake()
        {
            if (_cameraController == null)
                return;

            if (cameraShakeData == null)
                return;

            _cameraController.Shake(
                cameraShakeData
            );
        }

        private void PlayMuzzleFlash()
        {
            if (muzzleFlash == null)
                return;

            muzzleFlash.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            muzzleFlash.Play();
        }

        private void PlayFireAnimation()
        {
            if (animator == null)
                return;

            animator.SetTrigger(fireTrigger);
        }
    }
}