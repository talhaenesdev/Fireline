using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using Modules.PlayerModule.HudScreenModule.Signals;
using UnityEngine;

namespace Modules.PlayerModule.HudScreenModule.ViewsMediators
{
    public class HudScreenMediator : IMediator
    {
        [Inject] private HudScreenView _view { get; set; }
        [InjectSignal] private HudScreenSignals _signals { get; set; }

        public void OnRegister()
        {
            _view.ShowCompleted += OnScreenShown;
            _view.HideCompleted += OnScreenHidden;
        }

        public void OnRemove()
        {
            _view.ShowCompleted -= OnScreenShown;
            _view.HideCompleted -= OnScreenHidden;
        }

        private void OnScreenShown(IScreenBody screen)
        {
            _signals.Incoming.ApplyHealth.AddListener(_view.ShowHealth);
            _signals.Incoming.ApplyAmmo.AddListener(_view.ShowAmmo);
            _view.TouchMoved += OnTouchMoved;
            _view.TouchFireChanged += OnTouchFireChanged;
            _view.TouchReloadPressed += OnTouchReloadPressed;
        }

        private void OnScreenHidden(IScreenBody screen)
        {
            _signals.Incoming.ApplyHealth.RemoveListener(_view.ShowHealth);
            _signals.Incoming.ApplyAmmo.RemoveListener(_view.ShowAmmo);
            _view.TouchMoved -= OnTouchMoved;
            _view.TouchFireChanged -= OnTouchFireChanged;
            _view.TouchReloadPressed -= OnTouchReloadPressed;
        }

        private void OnTouchMoved(Vector2 move) => _signals.Outgoing.TouchMove.Dispatch(move);
        private void OnTouchFireChanged(bool isHeld, bool wasPressed) => _signals.Outgoing.TouchFire.Dispatch(isHeld, wasPressed);
        private void OnTouchReloadPressed() => _signals.Outgoing.TouchReload.Dispatch();
    }
}
