using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.NetworkSessionModule.Services;

namespace Modules.MatchModule.Controllers
{
    /// <summary>The match's rules run on the server; on a client the flow ends here.</summary>
    internal class RequireServerCommand : Command
    {
        [Inject] private INetworkSessionService _session { get; set; }

        public override void Execute()
        {
            if (_session.IsServer)
                return;

            Retain();
            Stop();
        }
    }
}
