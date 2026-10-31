using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.WeaponModule.Models;
using UnityEngine;

namespace Modules.WeaponModule.Controllers
{
    internal class StoreTriggerCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam(0)] private bool _isHeld { get; set; }
        [SignalParam(1)] private bool _wasPressed { get; set; }

        public override void Execute()
        {
            _model.IsTriggerHeld = _isHeld;

            // A press waits for the next frame's fire step, which consumes it.
            if (_wasPressed)
                _model.WasTriggerPressed = true;
        }
    }

    internal class StoreAimCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam] private Vector3 _point { get; set; }

        public override void Execute()
        {
            _model.AimPoint = _point;
            _model.HasAim = true;
        }
    }

    /// <summary>A bought upgrade: never faster than a fifth of the weapon's own rate.</summary>
    internal class ImproveFireRateCommand : Command
    {
        private const float FASTEST = 0.2f;

        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam] private float _amount { get; set; }

        public override void Execute() =>
            _model.FireRateMultiplier = Mathf.Max(FASTEST, _model.FireRateMultiplier * (1f - _amount));
    }

    internal class ImproveReloadCommand : Command
    {
        private const float FASTEST = 0.25f;

        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam] private float _amount { get; set; }

        public override void Execute() =>
            _model.ReloadMultiplier = Mathf.Max(FASTEST, _model.ReloadMultiplier * (1f - _amount));
    }

    internal class StoreArmedCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam] private bool _isArmed { get; set; }

        public override void Execute()
        {
            _model.IsArmed = _isArmed;

            if (!_isArmed)
                _model.IsTriggerHeld = false;
        }
    }
}
