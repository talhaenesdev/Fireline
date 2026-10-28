using System.Collections.Generic;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Entities;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;
using UnityEngine;

namespace Modules.PlayerModule.Controllers
{
    internal static class Revive
    {
        public const float RANGE = 2.5f;
        public const float DURATION = 3f;
        public const float HEALTH_SHARE = 0.5f;

        // A little slack on the server, so a revive does not drop on the edge of the range.
        public const float SERVER_RANGE = RANGE + 0.75f;

        public static bool IsNear(IPlayerAvatar a, IPlayerAvatar b, float range)
        {
            Vector3 offset = a.Transform.position - b.Transform.position;
            offset.y = 0f;
            return offset.sqrMagnitude <= range * range;
        }
    }

    /// <summary>
    /// The local player's revive, every frame: the nearest downed teammate in reach, and how the
    /// revive under way is going. Players on opposite sides do not revive each other, so only a
    /// match without friendly fire has it.
    /// </summary>
    internal class TrackReviveCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }

        public override void Execute()
        {
            IPlayerAvatar local = _model.LocalAvatar;
            IPlayerAvatar target = null;

            if (local != null && _model.IsLocalAlive && !_model.IsFriendlyFire)
            {
                float best = Revive.RANGE * Revive.RANGE;
                foreach (IPlayerAvatar avatar in _model.Avatars.Values)
                {
                    if (avatar == local || !avatar.IsDead)
                        continue;

                    Vector3 offset = avatar.Transform.position - local.Transform.position;
                    offset.y = 0f;
                    if (offset.sqrMagnitude > best)
                        continue;

                    best = offset.sqrMagnitude;
                    target = avatar;
                }
            }

            // Walking off, or the teammate getting up, ends the revive the local player was doing.
            if (target != _model.ReviveTarget)
                _model.ReviveEndTime = 0f;

            _model.ReviveTarget = target;

            float progress = -1f;
            if (target != null && _model.ReviveEndTime > 0f)
                progress = Mathf.Clamp01(1f - (_model.ReviveEndTime - Time.time) / Revive.DURATION);

            _signals.Outgoing.ReviveStateChanged.Dispatch(target != null, target != null ? target.Transform.position : Vector3.zero, progress);
        }
    }

    /// <summary>The local player starts reviving the teammate it stands by, and asks the server to.</summary>
    internal class StartReviveCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        public override void Execute()
        {
            if (_model.ReviveTarget == null || _model.LocalAvatar == null || _model.ReviveEndTime > 0f)
                return;

            _model.ReviveEndTime = Time.time + Revive.DURATION;
            _model.LocalAvatar.RequestRevive(_model.ReviveTarget.ClientId);
        }
    }

    /// <summary>Server: a living player next to a downed one starts a revive.</summary>
    internal class BeginServerReviveCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        [SignalParam(0)] private ulong _reviverClientId { get; set; }
        [SignalParam(1)] private ulong _targetClientId { get; set; }

        public override void Execute()
        {
            if (_model.IsFriendlyFire
                || !_model.TryGet(_reviverClientId, out IPlayerAvatar reviver)
                || !_model.TryGet(_targetClientId, out IPlayerAvatar target)
                || !reviver.IsServer || reviver.IsDead || !target.IsDead
                || !Revive.IsNear(reviver, target, Revive.SERVER_RANGE))
                return;

            _model.Revives[_reviverClientId] = (_targetClientId, Time.time + Revive.DURATION);
        }
    }

    /// <summary>
    /// Server, every frame: a revive whose reviver fell or walked off is dropped, and one that has
    /// run its time puts the downed player back on their feet with half their health.
    /// </summary>
    internal class ResolveRevivesCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }

        public override void Execute()
        {
            if (_model.Revives.Count == 0)
                return;

            var done = new List<ulong>();
            foreach (KeyValuePair<ulong, (ulong target, float endTime)> revive in _model.Revives)
            {
                bool valid = _model.TryGet(revive.Key, out IPlayerAvatar reviver)
                             && _model.TryGet(revive.Value.target, out IPlayerAvatar target)
                             && !reviver.IsDead && target.IsDead
                             && Revive.IsNear(reviver, target, Revive.SERVER_RANGE);

                if (!valid)
                {
                    done.Add(revive.Key);
                    continue;
                }

                if (Time.time < revive.Value.endTime)
                    continue;

                _model.TryGet(revive.Value.target, out IPlayerAvatar revived);
                revived.SetHealthServer(revived.MaxHealth * Revive.HEALTH_SHARE);
                revived.SetDeadServer(false);
                done.Add(revive.Key);
                _signals.Outgoing.PlayerRevived.Dispatch(revive.Value.target);
            }

            foreach (ulong reviver in done)
                _model.Revives.Remove(reviver);
        }
    }

    /// <summary>Server: a bought heal, up to full health. The dead are not healed back to life.</summary>
    internal class HealCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private float _amount { get; set; }

        public override void Execute()
        {
            if (_model.TryGet(_clientId, out IPlayerAvatar avatar) && avatar.IsServer && !avatar.IsDead)
                avatar.SetHealthServer(Mathf.Min(avatar.MaxHealth, avatar.Health + _amount));
        }
    }

    /// <summary>Server: a bought shield, on top of whatever shield is left.</summary>
    internal class AddShieldCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private float _amount { get; set; }

        public override void Execute()
        {
            if (_model.TryGet(_clientId, out IPlayerAvatar avatar) && avatar.IsServer && !avatar.IsDead)
                avatar.SetShieldServer(avatar.Shield + _amount);
        }
    }

    /// <summary>The HUD shows the local player's shield and nobody else's.</summary>
    internal class PublishLocalShieldCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private float _shield { get; set; }

        public override void Execute()
        {
            if (_model.LocalAvatar != null && _model.LocalAvatar.ClientId == _clientId)
                _signals.Outgoing.LocalShieldChanged.Dispatch(_shield);
        }
    }
}
