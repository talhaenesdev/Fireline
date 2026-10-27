using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Entities;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;
using UnityEngine;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>The server takes a hit off a living player's health, and a player at zero dies.</summary>
    internal class ApplyDamageCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }

        [SignalParam(0)] private ulong _targetClientId { get; set; }
        [SignalParam(1)] private ulong _attackerClientId { get; set; }
        [SignalParam] private int _damage { get; set; }

        public override void Execute()
        {
            if (_damage <= 0 || !_model.TryGet(_targetClientId, out IPlayerAvatar target))
                return;

            if (!target.IsServer || target.IsDead)
                return;

            // Another player's round, with players on one side: no harm done.
            if (!_model.IsFriendlyFire && _model.TryGet(_attackerClientId, out _))
                return;

            float health = Mathf.Max(0f, target.Health - _damage);
            target.SetHealthServer(health);
            target.PlayHitEffectServer();

            if (health > 0f)
                return;

            target.SetDeadServer(true);
            _signals.Outgoing.PlayerDied.Dispatch(_targetClientId, _attackerClientId);
        }
    }
}
