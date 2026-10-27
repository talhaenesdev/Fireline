using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.MatchModule.Models;
using Modules.MatchModule.Shared.Data.ValueObjects;
using Unity.Netcode;

namespace Modules.MatchModule.Controllers
{
    /// <summary>A new client starts on the board with nothing.</summary>
    internal class AddScoreCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            ScoreEntryVO entry = _model.GetScore(_clientId);
            _model.SetScore(entry);
            _model.Scoreboard?.SetEntryServer(entry);
        }
    }

    internal class RemoveScoreCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            _model.RemoveScore(_clientId);
            _model.Scoreboard?.RemoveEntryServer(_clientId);
        }
    }

    /// <summary>The victim gains a death; the killer gains a kill unless it was the victim.</summary>
    internal class RecordKillCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam(0)] private ulong _victimClientId { get; set; }
        [SignalParam(1)] private ulong _killerClientId { get; set; }

        public override void Execute()
        {
            ScoreEntryVO victim = _model.GetScore(_victimClientId);
            Write(new ScoreEntryVO(victim.ClientId, victim.PlayerName, victim.Kills, victim.Deaths + 1));

            // Killing yourself scores nothing, and neither does a zombie: it is no client.
            if (_killerClientId == _victimClientId || !NetworkManager.Singleton.ConnectedClients.ContainsKey(_killerClientId))
                return;

            ScoreEntryVO killer = _model.GetScore(_killerClientId);
            Write(new ScoreEntryVO(killer.ClientId, killer.PlayerName, killer.Kills + 1, killer.Deaths));
        }

        private void Write(ScoreEntryVO entry)
        {
            _model.SetScore(entry);
            _model.Scoreboard?.SetEntryServer(entry);
        }
    }

    /// <summary>A zombie a player shot down counts as that player's kill.</summary>
    internal class AddZombieKillCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam] private ulong _killerClientId { get; set; }

        public override void Execute()
        {
            if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(_killerClientId))
                return;

            ScoreEntryVO killer = _model.GetScore(_killerClientId);
            var entry = new ScoreEntryVO(killer.ClientId, killer.PlayerName, killer.Kills + 1, killer.Deaths);
            _model.SetScore(entry);
            _model.Scoreboard?.SetEntryServer(entry);
        }
    }

    internal class UpdateScoreNameCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private string _playerName { get; set; }

        public override void Execute()
        {
            ScoreEntryVO score = _model.GetScore(_clientId);
            var entry = new ScoreEntryVO(score.ClientId, _playerName, score.Kills, score.Deaths);
            _model.SetScore(entry);
            _model.Scoreboard?.SetEntryServer(entry);
        }
    }
}
