using FireLine.Scripts.Player.Controller;
using UnityEngine;

namespace FireLine.Scripts.Network.Controller
{
    public class NetworkPlayerDeathAnimationController : MonoBehaviour
    {
        private NetworkPlayerHealth _playerHealth;
        private PlayerAnimationController _animationController;

        private void Awake()
        {
            _playerHealth =
                GetComponent<NetworkPlayerHealth>();

            _animationController =
                GetComponent<PlayerAnimationController>();

            if (_playerHealth == null)
            {
                Debug.LogError(
                    "[DEATH ANIMATION] " +
                    "NetworkPlayerHealth not found!"
                );
            }

            if (_animationController == null)
            {
                Debug.LogError(
                    "[DEATH ANIMATION] " +
                    "PlayerAnimationController not found!"
                );
            }
        }

        private void OnEnable()
        {
            if (_playerHealth == null)
                return;

            _playerHealth.DeathStateChanged +=
                OnDeathStateChanged;
        }

        private void OnDisable()
        {
            if (_playerHealth == null)
                return;

            _playerHealth.DeathStateChanged -=
                OnDeathStateChanged;
        }

        private void OnDeathStateChanged(bool isDead)
        {
            if (_animationController == null)
                return;

            _animationController.SetDeath(isDead);
        }
    }
}