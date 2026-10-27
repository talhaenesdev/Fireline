using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Models;
using UnityEngine;

namespace Modules.PlayerModule.Controllers
{
    internal class StoreMoveInputCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        [SignalParam] private Vector2 _move { get; set; }

        public override void Execute() => _model.MoveInput = _move;
    }
}
