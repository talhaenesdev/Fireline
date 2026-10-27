using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.MatchModule.Models;
using Unity.Netcode;

namespace Modules.MatchModule.Controllers
{
    /// <summary>A client that joins mid-match gets a player, unless it has one or the arena is not up yet.</summary>
    internal class SpawnPlayerCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            // Before the arena registers or the scene launches, SpawnConnectedPlayersCommand picks this client up.
            if (_model.Arena == null || !_model.IsLaunched)
                return;

            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(_clientId, out NetworkClient client) && client.PlayerObject == null)
                PlayerSpawning.Spawn(_model.Arena, _clientId);
        }
    }
}
