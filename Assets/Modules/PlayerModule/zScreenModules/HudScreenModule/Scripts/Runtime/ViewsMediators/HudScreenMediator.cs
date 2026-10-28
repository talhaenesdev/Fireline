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
            _signals.Incoming.ApplyWave.AddListener(_view.ShowWave);
            _signals.Incoming.ApplyShield.AddListener(_view.ShowShield);
            _signals.Incoming.ApplyWallet.AddListener(_view.ShowWallet);
            _signals.Incoming.ApplyRevive.AddListener(_view.ShowRevive);
            _view.ReviveClicked += OnReviveClicked;
            _view.PlaceTurretClicked += OnPlaceTurretClicked;
            _signals.Incoming.ApplyTurretPlacement.AddListener(_view.ShowTurretPlacement);
            _view.TouchMoved += OnTouchMoved;
            _view.TouchFireChanged += OnTouchFireChanged;
            _view.TouchReloadPressed += OnTouchReloadPressed;
        }

        private void OnScreenHidden(IScreenBody screen)
        {
            _signals.Incoming.ApplyHealth.RemoveListener(_view.ShowHealth);
            _signals.Incoming.ApplyAmmo.RemoveListener(_view.ShowAmmo);
            _signals.Incoming.ApplyWave.RemoveListener(_view.ShowWave);
            _signals.Incoming.ApplyShield.RemoveListener(_view.ShowShield);
            _signals.Incoming.ApplyWallet.RemoveListener(_view.ShowWallet);
            _signals.Incoming.ApplyRevive.RemoveListener(_view.ShowRevive);
            _view.ReviveClicked -= OnReviveClicked;
            _view.PlaceTurretClicked -= OnPlaceTurretClicked;
            _signals.Incoming.ApplyTurretPlacement.RemoveListener(_view.ShowTurretPlacement);
            _view.TouchMoved -= OnTouchMoved;
            _view.TouchFireChanged -= OnTouchFireChanged;
            _view.TouchReloadPressed -= OnTouchReloadPressed;
        }

        private void OnTouchMoved(Vector2 move) => _signals.Outgoing.TouchMove.Dispatch(move);
        private void OnTouchFireChanged(bool isHeld, bool wasPressed) => _signals.Outgoing.TouchFire.Dispatch(isHeld, wasPressed);
        private void OnTouchReloadPressed() => _signals.Outgoing.TouchReload.Dispatch();
        private void OnReviveClicked() => _signals.Outgoing.ReviveClicked.Dispatch();
        private void OnPlaceTurretClicked() => _signals.Outgoing.PlaceTurretClicked.Dispatch();
    }
}
