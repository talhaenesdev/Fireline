using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.HudScreenModule.Models;
using UnityEngine;

namespace Modules.PlayerModule.HudScreenModule.Controllers
{
    internal class RememberHealthCommand : Command
    {
        [Inject] private IHudScreenModel _model { get; set; }

        [SignalParam(0)] private float _health { get; set; }
        [SignalParam(1)] private float _maxHealth { get; set; }

        public override void Execute()
        {
            _model.Health = _health;
            _model.MaxHealth = _maxHealth;
        }
    }

    internal class RememberAmmoCommand : Command
    {
        [Inject] private IHudScreenModel _model { get; set; }

        [SignalParam(0)] private int _ammo { get; set; }
        [SignalParam(1)] private int _magazineSize { get; set; }

        public override void Execute()
        {
            _model.Ammo = _ammo;
            _model.MagazineSize = _magazineSize;
        }
    }

    internal class RememberShieldCommand : Command
    {
        [Inject] private IHudScreenModel _model { get; set; }

        [SignalParam] private float _shield { get; set; }

        public override void Execute() => _model.Shield = _shield;
    }

    internal class RememberWalletCommand : Command
    {
        [Inject] private IHudScreenModel _model { get; set; }

        [SignalParam(0)] private int _money { get; set; }
        [SignalParam(1)] private int _carried { get; set; }

        public override void Execute()
        {
            _model.Money = _money;
            _model.Carried = _carried;
            _model.HasWallet = true;
        }
    }

    internal class RememberWaveCommand : Command
    {
        [Inject] private IHudScreenModel _model { get; set; }

        [SignalParam(0)] private int _wave { get; set; }
        [SignalParam] private float _secondsUntilWave { get; set; }
        [SignalParam(1)] private int _zombiesLeft { get; set; }

        public override void Execute()
        {
            _model.Wave = _wave;
            _model.ZombiesLeft = _zombiesLeft;
            _model.WaveStartTime = Time.time + _secondsUntilWave;
        }
    }
}
