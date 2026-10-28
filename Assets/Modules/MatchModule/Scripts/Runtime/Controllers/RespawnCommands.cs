using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.Provider.Coroutine;
using System.Linq;
using Modules.MatchModule.Models;
using Modules.MatchModule.Shared.Enums;
using Unity.Netcode;

namespace Modules.MatchModule.Controllers
{
    /// <summary>
    /// A zombie match has no coming back: the dead player stays down, and once nobody is left
    /// standing the match is over. Deathmatch goes on to the respawn.
    /// </summary>
    internal class ResolveZombieDeathCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam(0)] private ulong _victimClientId { get; set; }

        public override void Execute()
        {
            if (_model.Mode != GameMode.Zombie)
                return;

            _model.DownedClients.Add(_victimClientId);
            ZombieGameOver.EndIfEveryoneIsDown(_model);

            Retain();
            Stop();
        }
    }

    /// <summary>The last player standing leaving ends a zombie match as surely as falling does.</summary>
    internal class CheckZombieGameOverCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            _model.DownedClients.Remove(_clientId);

            if (_model.Mode == GameMode.Zombie)
                ZombieGameOver.EndIfEveryoneIsDown(_model);
        }
    }

    /// <summary>A revived player is standing again, and counts against the game being over.</summary>
    internal class MarkPlayerStandingCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute() => _model.DownedClients.Remove(_clientId);
    }

    internal static class ZombieGameOver
    {
        public static void EndIfEveryoneIsDown(IMatchModel model)
        {
            var standing = NetworkManager.Singleton.ConnectedClientsIds.Where(id => !model.DownedClients.Contains(id));
            if (!standing.Any())
                model.Scoreboard?.SetGameOverServer();
        }
    }

    internal class WaitRespawnDelayCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }
        [Inject] private ICoroutineProvider _coroutineProvider { get; set; }

        public override void Execute()
        {
            Retain();
            _coroutineProvider.WaitForSeconds(_model.Arena != null ? _model.Arena.RespawnDelay : 0f, () => Release());
        }
    }

    /// <summary>The dead player's object is replaced with a fresh one, if that client is still here.</summary>
    internal class RespawnPlayerCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam(0)] private ulong _victimClientId { get; set; }

        public override void Execute()
        {
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(_victimClientId, out NetworkClient client))
                return;

            if (client.PlayerObject != null && client.PlayerObject.IsSpawned)
                client.PlayerObject.Despawn(true);

            PlayerSpawning.Spawn(_model.Arena, _victimClientId);
        }
    }
}
