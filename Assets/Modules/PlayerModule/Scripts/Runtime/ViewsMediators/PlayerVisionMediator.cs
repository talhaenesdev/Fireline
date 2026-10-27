using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Unity.Netcode;

namespace Modules.PlayerModule.ViewsMediators
{
    public class PlayerVisionMediator : IMediator
    {
        [Inject] private PlayerVisionView _view { get; set; }

        public void OnRegister() =>
            _view.Enable(_view.TryGetComponent(out NetworkObject networkObject) && networkObject.IsOwner);

        public void OnRemove() => _view.Enable(false);
    }
}
