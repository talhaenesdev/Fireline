using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.PlayerModule.Signals;
using Unity.Netcode;
using UnityEngine;

namespace Modules.PlayerModule.ViewsMediators
{
    public class PlayerAimMediator : IMediator
    {
        [Inject] private PlayerAimView _view { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }
        [InjectSignal] private PlayerInternalSignals _internalSignals { get; set; }

        private bool _isOwner;

        public void OnRegister()
        {
            _isOwner = _view.TryGetComponent(out NetworkObject networkObject) && networkObject.IsOwner;
            _view.Enable(_isOwner);
            _view.AimChanged += OnAimChanged;
            _internalSignals.FaceLocalAvatar.AddListener(OnFaceLocalAvatar);
        }

        public void OnRemove()
        {
            _view.AimChanged -= OnAimChanged;
            _internalSignals.FaceLocalAvatar.RemoveListener(OnFaceLocalAvatar);
        }

        private void OnFaceLocalAvatar(Vector3 direction)
        {
            if (_isOwner)
                _view.Face(direction);
        }

        private void OnAimChanged(Vector3 point)
        {
            _internalSignals.AimPointChanged.Dispatch(point);
            _signals.Outgoing.AimChanged.Dispatch(point);
        }
    }
}
