using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Models;
using UnityEngine;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>A phone or tablet plays with on-screen controls and lets the avatar aim by itself.</summary>
    internal class DetectTouchInputCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        public override void Execute() => _model.IsTouchInput = Application.isMobilePlatform;
    }
}
