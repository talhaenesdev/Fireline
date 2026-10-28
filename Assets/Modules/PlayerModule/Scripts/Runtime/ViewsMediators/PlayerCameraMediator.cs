using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.PlayerModule.Signals;
using Unity.Netcode;

namespace Modules.PlayerModule.ViewsMediators
{
    public class PlayerCameraMediator : IMediator
    {
        [Inject] private PlayerCameraView _view { get; set; }
        [InjectSignal] private PlayerInternalSignals _internalSignals { get; set; }

        public void OnRegister()
        {
            if (!_view.TryGetComponent(out NetworkObject networkObject) || !networkObject.IsOwner)
                return;

            _view.Follow();
            _internalSignals.ShakeLocalCamera.AddListener(_view.Shake);
            _internalSignals.ApplyLocalCamera.AddListener(_view.Apply);
            _internalSignals.LocalCameraRegistered.Dispatch(_view.Settings);
        }

        public void OnRemove()
        {
            _internalSignals.ShakeLocalCamera.RemoveListener(_view.Shake);
            _internalSignals.ApplyLocalCamera.RemoveListener(_view.Apply);
        }
    }
}
