using System.Collections.Generic;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.PoolModule.Services;
using Modules.PlayerModule.Entities;
using Modules.PlayerModule.Enums;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Shared.Data.ValueObjects;
using Modules.PlayerModule.Shared.Enums;
using Modules.PlayerModule.Signals;
using UnityEngine;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>Server: a bought perk goes onto the buyer's avatar, where every process reads it.</summary>
    internal class ApplyPerkCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private int _perk { get; set; }
        [SignalParam] private float _amount { get; set; }

        public override void Execute()
        {
            if (!_model.TryGet(_clientId, out IPlayerAvatar avatar) || !avatar.IsServer)
                return;

            switch ((PlayerPerk) _perk)
            {
                case PlayerPerk.MaxHealth:
                    avatar.RaiseMaxHealthServer(_amount);
                    break;
                case PlayerPerk.ReviveSpeed:
                    avatar.ScaleReviveServer(_amount);
                    break;
                case PlayerPerk.MoveSpeed:
                    avatar.ScaleSpeedServer(_amount);
                    break;
            }
        }
    }

    /// <summary>The point the local player aims at is kept, for a ping.</summary>
    internal class StoreAimPointCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        [SignalParam] private Vector3 _point { get; set; }

        public override void Execute() => _model.AimPoint = _point;
    }

    /// <summary>The local player pings the point it aims at.</summary>
    internal class RequestPingCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        public override void Execute()
        {
            if (_model.LocalAvatar != null && _model.IsLocalAlive)
                _model.LocalAvatar.RequestPing(new Vector3(_model.AimPoint.x, 0f, _model.AimPoint.z));
        }
    }

    /// <summary>Server: a player's ping goes out to everyone.</summary>
    internal class BroadcastPingCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private Vector3 _point { get; set; }

        public override void Execute()
        {
            if (_model.TryGet(_clientId, out IPlayerAvatar avatar))
                avatar.ShowPingServer(_point);
        }
    }

    /// <summary>A ping shows where it was made, from the pool, and the HUD points at it for a while.</summary>
    internal class ShowPingCommand : Command
    {
        private const string PING_POOL_KEY = "PingMarker";
        private const float PING_TIME = 6f;

        [Inject] private IPlayerModel _model { get; set; }
        [Inject] private IPoolService _poolService { get; set; }

        [SignalParam] private Vector3 _point { get; set; }

        public override void Execute()
        {
            _poolService.Get<PingMarker>(PING_POOL_KEY)?.Show(_point, PING_TIME);
            _model.Pings.Add((_point, Time.time + PING_TIME));
        }
    }

    /// <summary>A few times a second: what the HUD may point at off the screen.</summary>
    internal class CollectMarkersCommand : Command
    {
        private const float INTERVAL = 0.1f;
        private const string ZOMBIE_TAG = "Zombie";

        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }

        private readonly List<MarkerVO> _markers = new();

        public override void Execute()
        {
            if (Time.time < _model.NextMarkersTime || _model.LocalAvatar == null)
                return;

            _model.NextMarkersTime = Time.time + INTERVAL;
            _markers.Clear();

            foreach (GameObject zombie in GameObject.FindGameObjectsWithTag(ZOMBIE_TAG))
                _markers.Add(new MarkerVO(zombie.transform.position, MarkerKind.Zombie));

            foreach (IPlayerAvatar avatar in _model.Avatars.Values)
            {
                if (avatar != _model.LocalAvatar && avatar.IsDead)
                    _markers.Add(new MarkerVO(avatar.Transform.position, MarkerKind.DownedTeammate));
            }

            _model.Pings.RemoveAll(ping => ping.until < Time.time);
            foreach (var ping in _model.Pings)
                _markers.Add(new MarkerVO(ping.point, MarkerKind.Ping));

            _signals.Outgoing.MarkersChanged.Dispatch(_markers.ToArray());
        }
    }
}
