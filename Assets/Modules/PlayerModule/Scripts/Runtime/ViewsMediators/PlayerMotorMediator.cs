using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.PlayerModule.Signals;
using Unity.Netcode;
using UnityEngine;

namespace Modules.PlayerModule.ViewsMediators
{
    public class PlayerMotorMediator : IMediator
    {
        [Inject] private PlayerMotorView _view { get; set; }
        [InjectSignal] private PlayerInternalSignals _internalSignals { get; set; }

        private NetworkObject _networkObject;

        public void OnRegister()
        {
            _networkObject = _view.GetComponent<NetworkObject>();
            _internalSignals.MoveLocalAvatar.AddListener(OnMoveLocalAvatar);
            _internalSignals.DeathChanged.AddListener(OnDeathChanged);
        }

        public void OnRemove()
        {
            _internalSignals.MoveLocalAvatar.RemoveListener(OnMoveLocalAvatar);
            _internalSignals.DeathChanged.RemoveListener(OnDeathChanged);
        }

        // Every avatar hears the local step; the one that is local takes it. Remote avatars are
        // moved by their NetworkTransform.
        private void OnMoveLocalAvatar(Vector3 step, Vector2 input, bool isSprinting)
        {
            if (_networkObject != null && _networkObject.IsOwner)
                _view.Move(step, input, isSprinting);
        }

        private void OnDeathChanged(ulong clientId, bool isDead)
        {
            if (_networkObject != null && _networkObject.OwnerClientId == clientId)
                _view.ShowDeath(isDead);
        }
    }
}
