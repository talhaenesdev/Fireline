using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EnemyModule.Signals;
using UnityEngine;

namespace Modules.EnemyModule.Controllers
{
    /// <summary>Tells the rest of the game something heavy happened here, so a camera nearby can shake.</summary>
    internal class AnnounceRumbleCommand : Command
    {
        [InjectSignal] private EnemySignals _signals { get; set; }

        [SignalParam] private Vector3 _position { get; set; }

        public override void Execute() => _signals.Outgoing.Rumbled.Dispatch(_position);
    }
}
