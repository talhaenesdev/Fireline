using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.WeaponModule.Entities;
using UnityEngine;

namespace Modules.WeaponModule.Controllers
{
    internal class BroadcastFireCommand : Command
    {
        [SignalParam] private IWeapon _shooter { get; set; }
        [SignalParam(0)] private Vector3 _position { get; set; }

        public override void Execute() => _shooter.BroadcastFire(_position);
    }
}
