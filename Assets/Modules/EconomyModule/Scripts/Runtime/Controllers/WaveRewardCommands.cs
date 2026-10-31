using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EconomyModule.Data.UnityObjects;
using Modules.EconomyModule.Models;
using Modules.EconomyModule.Signals;
using Unity.Netcode;

namespace Modules.EconomyModule.Controllers
{
    /// <summary>Every process tells its player what the cleared wave paid.</summary>
    internal class AnnounceWaveRewardCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }
        [InjectSignal] private EconomySignals _signals { get; set; }

        [SignalParam] private int _wave { get; set; }

        public override void Execute()
        {
            CD_Loot loot = _model.Board?.Loot;
            if (loot != null)
                _signals.Outgoing.WaveRewardAnnounced.Dispatch(_wave, loot.WaveReward(_wave));
        }
    }

    /// <summary>Server: every connected player gets the cleared wave's reward.</summary>
    internal class GrantWaveRewardCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        [SignalParam] private int _wave { get; set; }

        public override void Execute()
        {
            CD_Loot loot = _model.Board?.Loot;
            if (loot == null)
                return;

            int reward = loot.WaveReward(_wave);
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                _model.Money[clientId] = EconomyWallets.MoneyOf(_model, clientId) + reward;
                EconomyWallets.Publish(_model, clientId);
            }
        }
    }
}
