using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.WeaponModule.Signals;
using UnityEngine;

namespace Modules.WeaponModule.ViewsMediators
{
    public class NetworkBulletMediator : IMediator
    {
        [Inject] private NetworkBulletView _view { get; set; }
        [InjectSignal] private WeaponInternalSignals _internalSignals { get; set; }

        public void OnRegister()
        {
            _view.Hit += OnHit;
            _view.ImpactShown += OnImpactShown;
        }

        public void OnRemove()
        {
            _view.Hit -= OnHit;
            _view.ImpactShown -= OnImpactShown;
        }

        private void OnHit(Collider other) => _internalSignals.BulletHit.Dispatch(_view, other);

        private void OnImpactShown(Vector3 point) => _internalSignals.ImpactShown.Dispatch(point);
    }
}
