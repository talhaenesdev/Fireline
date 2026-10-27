using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.WeaponModule.Entities;
using Modules.WeaponModule.Signals;
using Unity.Netcode;
using UnityEngine;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>
    /// What a server round touched decides what happens to it. Its own shooter is passed through,
    /// another player is hit, another networked object - a zombie - is hit, and anything else is a wall.
    /// </summary>
    internal class ResolveBulletHitCommand : Command
    {
        [InjectSignal] private WeaponSignals _signals { get; set; }

        [SignalParam] private INetworkBullet _bullet { get; set; }
        [SignalParam] private Collider _other { get; set; }

        public override void Execute()
        {
            NetworkObject target = _other.GetComponentInParent<NetworkObject>();

            if (target != null && target.IsPlayerObject)
            {
                if (target.OwnerClientId == _bullet.ShooterClientId)
                    return;

                _signals.Outgoing.PlayerHit.Dispatch(target.OwnerClientId, _bullet.Damage, _bullet.ShooterClientId);
                _bullet.Despawn();
                return;
            }

            if (target != null)
            {
                _signals.Outgoing.ObjectHit.Dispatch(target.NetworkObjectId, _bullet.Damage, _bullet.ShooterClientId);
                _bullet.Despawn();
                return;
            }

            _bullet.ShowImpact(_bullet.Position);
            _bullet.Despawn();
        }
    }
}
