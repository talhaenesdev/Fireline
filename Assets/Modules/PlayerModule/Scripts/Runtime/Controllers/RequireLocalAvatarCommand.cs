using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Entities;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>The rest of the flow is about the player this process controls; a remote avatar ends it here.</summary>
    internal class RequireLocalAvatarCommand : Command
    {
        [SignalParam] private IPlayerAvatar _avatar { get; set; }

        public override void Execute()
        {
            if (_avatar.IsOwner)
                return;

            Retain();
            Stop();
        }
    }
}
