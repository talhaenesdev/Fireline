using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.MatchModule.Models;
using Unity.Netcode;

namespace Modules.MatchModule.Controllers
{
    /// <summary>Everybody who came over from the lobby gets a player once the arena is known.</summary>
    internal class SpawnConnectedPlayersCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        public override void Execute()
        {
            // Players spawn once the arena is known and the scene has launched, whichever comes last.
            if (_model.Arena == null || !_model.IsLaunched)
                return;

            NetworkManager manager = NetworkManager.Singleton;

            foreach (ulong clientId in manager.ConnectedClientsIds)
            {
                if (manager.ConnectedClients.TryGetValue(clientId, out NetworkClient client) && client.PlayerObject == null)
                    PlayerSpawning.Spawn(_model.Arena, clientId);
            }
        }
    }
}
