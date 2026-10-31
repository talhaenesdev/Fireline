using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.PlayerModule.Signals;
using Unity.Netcode;
using UnityEngine;

namespace Modules.PlayerModule.ViewsMediators
{
    public class PlayerInputMediator : IMediator
    {
        [Inject] private PlayerInputView _view { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }
        [InjectSignal] private PlayerInternalSignals _internalSignals { get; set; }

        private bool IsLocal => _view.TryGetComponent(out NetworkObject networkObject) && networkObject.IsOwner;

        public void OnRegister()
        {
            // A remote player's avatar carries the same component; only the owner reads a device.
            _view.Enable(IsLocal);

            _view.MoveChanged += OnMoveChanged;
            _view.SprintChanged += OnSprintChanged;
            _view.FireChanged += OnFireChanged;
            _view.ReloadPressed += OnReloadPressed;
            _view.PingPressed += OnPingPressed;
        }

        public void OnRemove()
        {
            _view.MoveChanged -= OnMoveChanged;
            _view.SprintChanged -= OnSprintChanged;
            _view.FireChanged -= OnFireChanged;
            _view.ReloadPressed -= OnReloadPressed;
            _view.PingPressed -= OnPingPressed;
        }

        private void OnMoveChanged(Vector2 move) => _internalSignals.MoveInput.Dispatch(move);
        private void OnSprintChanged(bool sprint) => _internalSignals.SprintInput.Dispatch(sprint);
        private void OnFireChanged(bool held, bool pressed) => _signals.Outgoing.FireInput.Dispatch(held, pressed);
        private void OnReloadPressed() => _signals.Outgoing.ReloadInput.Dispatch();
        private void OnPingPressed() => _internalSignals.PingPressed.Dispatch();
    }
}
