using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EconomyModule.Data.UnityObjects;
using Modules.EconomyModule.Enums;
using Modules.EconomyModule.Models;
using Modules.EconomyModule.Signals;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EconomyModule.Controllers
{
    /// <summary>Server: a kill earns its shooter experience.</summary>
    internal class CreditKillCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            CD_Progression progression = _model.Board?.Progression;
            if (progression != null)
                Progression.Add(_model, _clientId, progression.XpPerKill);
        }
    }

    /// <summary>Server: a cleared wave earns every player experience.</summary>
    internal class CreditWaveCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        public override void Execute()
        {
            CD_Progression progression = _model.Board?.Progression;
            if (progression == null)
                return;

            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
                Progression.Add(_model, clientId, progression.XpPerWave);
        }
    }

    /// <summary>Server: every player is paid, for an objective held.</summary>
    internal class PayEveryoneCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        [SignalParam] private int _amount { get; set; }

        public override void Execute()
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                _model.Money[clientId] = EconomyWallets.MoneyOf(_model, clientId) + _amount;
                EconomyWallets.Publish(_model, clientId);
            }
        }
    }

    /// <summary>
    /// The local player's experience this match is added to the total kept on this device as it comes
    /// in, so a match left half way still counts; the HUD shows the level it reaches.
    /// </summary>
    internal class SaveLocalXpCommand : Command
    {
        private const string TOTAL_XP_KEY = "Fireline.TotalXp";

        [Inject] private IEconomyModel _model { get; set; }
        [InjectSignal] private EconomySignals _signals { get; set; }

        public override void Execute()
        {
            NetworkManager manager = NetworkManager.Singleton;
            CD_Progression progression = _model.Board?.Progression;
            if (progression == null || manager == null || !manager.IsClient)
                return;

            _model.Board.TryGetXp(manager.LocalClientId, out int matchXp);
            int total = PlayerPrefs.GetInt(TOTAL_XP_KEY, 0);
            if (matchXp > _model.SavedMatchXp)
            {
                total += matchXp - _model.SavedMatchXp;
                _model.SavedMatchXp = matchXp;
                PlayerPrefs.SetInt(TOTAL_XP_KEY, total);
                PlayerPrefs.Save();
            }

            int level = progression.LevelOf(total, out int intoLevel, out int levelSize);
            _signals.Outgoing.LocalProgressChanged.Dispatch(level, intoLevel, levelSize);

            if (_model.HasReportedLevel || !_model.Board.IsSpawned)
                return;

            _model.HasReportedLevel = true;
            _model.Board.ReportLevel(level);
        }
    }

    /// <summary>Server: a player starts the match with money for the level they have, once.</summary>
    internal class PayForLevelCommand : Command
    {
        private const int MAX_LEVEL = 50;

        [Inject] private IEconomyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private int _level { get; set; }

        public override void Execute()
        {
            CD_Progression progression = _model.Board?.Progression;
            if (progression == null || !_model.PaidForLevel.Add(_clientId))
                return;

            _model.Money[_clientId] = EconomyWallets.MoneyOf(_model, _clientId) + Mathf.Clamp(_level, 0, MAX_LEVEL) * progression.StartingMoneyPerLevel;
            EconomyWallets.Publish(_model, _clientId);
        }
    }

    /// <summary>Every process takes away the blockers of the doors that were bought open.</summary>
    internal class ApplyDoorsCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        public override void Execute()
        {
            if (_model.Board == null)
                return;

            foreach (var zone in _model.Zones)
            {
                if (zone.Kind == EconomyZoneKind.Door)
                    zone.ShowOpen(_model.Board.IsDoorOpen(zone.Id));
            }
        }
    }

    internal static class Progression
    {
        public static void Add(IEconomyModel model, ulong clientId, int xp)
        {
            model.Xp[clientId] = (model.Xp.TryGetValue(clientId, out int current) ? current : 0) + xp;
            model.Board.SetXpServer(clientId, model.Xp[clientId]);
        }
    }
}
