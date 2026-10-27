using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.Provider.Coroutine;
using Modules.MatchModule.Models;
using Unity.Netcode;

namespace Modules.MatchModule.Controllers
{
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
