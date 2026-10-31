using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using Modules.LobbyModule.Models;
using Modules.LobbyModule.Shared.Constants;
using Modules.LobbyModule.Shared.Enums;
using Modules.MatchModule.Shared.Constants;
using Modules.MatchModule.Shared.Enums;
using Modules.NetworkSessionModule.Services;

namespace Modules.LobbyModule.Controllers
{
    /// <summary>
    /// The host writes the lobby's mode onto the session, where every player in it reads it and the
    /// match scene finds it. A new session starts on whatever mode the lobby shows.
    /// </summary>
    internal class WriteGameModeCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }

        public override async void Execute()
        {
            Retain();

            try
            {
                await _session.SetSessionPropertyAsync(MatchConstants.GAME_MODE_PROPERTY, _model.Mode.ToString());
                await _session.SetSessionPropertyAsync(MatchConstants.DIFFICULTY_PROPERTY, _model.Difficulty.ToString());
                await _session.SetSessionPropertyAsync(LobbyConstants.MAP_PROPERTY, _model.Map.ToString());
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"WriteGameModeCommand threw: {exception}");
            }

            Release();
        }
    }

    /// <summary>
    /// The next mode, in turn. Before a lobby exists the choice is the lobby's own and is written when
    /// the session is created; in a session only the host chooses, and the session carries it to everybody.
    /// </summary>
    internal class CycleGameModeCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }

        public override async void Execute()
        {
            Retain();

            GameMode next = _model.Mode == GameMode.Deathmatch ? GameMode.Zombie : GameMode.Deathmatch;

            if (!_model.Session.HasSession)
            {
                _model.Mode = next;
                Release();
                return;
            }

            if (!_model.Session.IsHost)
            {
                _model.Finish(null);
                Stop();
                return;
            }

            try
            {
                if (await _session.SetSessionPropertyAsync(MatchConstants.GAME_MODE_PROPERTY, next.ToString()))
                    _model.Mode = next;
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"CycleGameModeCommand threw: {exception}");
            }

            Release();
        }
    }

    /// <summary>The next difficulty, in turn, chosen the way the mode is: by the lobby before a session, by the host in one.</summary>
    internal class CycleDifficultyCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }

        public override async void Execute()
        {
            Retain();

            var next = (Difficulty) (((int) _model.Difficulty + 1) % 3);

            if (!_model.Session.HasSession)
            {
                _model.Difficulty = next;
                Release();
                return;
            }

            if (!_model.Session.IsHost)
            {
                _model.Finish(null);
                Stop();
                return;
            }

            try
            {
                if (await _session.SetSessionPropertyAsync(MatchConstants.DIFFICULTY_PROPERTY, next.ToString()))
                    _model.Difficulty = next;
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"CycleDifficultyCommand threw: {exception}");
            }

            Release();
        }
    }

    /// <summary>The next zombie map, in turn, chosen the way the mode is: by the lobby before a session, by the host in one.</summary>
    internal class CycleMapCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }

        public override async void Execute()
        {
            Retain();

            ZombieMap next = _model.Map == ZombieMap.Arena ? ZombieMap.Base : ZombieMap.Arena;

            if (!_model.Session.HasSession)
            {
                _model.Map = next;
                Release();
                return;
            }

            if (!_model.Session.IsHost)
            {
                _model.Finish(null);
                Stop();
                return;
            }

            try
            {
                if (await _session.SetSessionPropertyAsync(LobbyConstants.MAP_PROPERTY, next.ToString()))
                    _model.Map = next;
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"CycleMapCommand threw: {exception}");
            }

            Release();
        }
    }

    /// <summary>A client learns the host's choice from the session whenever the session changes.</summary>
    internal class ReadGameModeCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }

        public override void Execute()
        {
            if (Enum.TryParse(_session.GetSessionProperty(MatchConstants.GAME_MODE_PROPERTY), out GameMode mode))
                _model.Mode = mode;

            if (Enum.TryParse(_session.GetSessionProperty(MatchConstants.DIFFICULTY_PROPERTY), out Difficulty difficulty))
                _model.Difficulty = difficulty;

            if (Enum.TryParse(_session.GetSessionProperty(LobbyConstants.MAP_PROPERTY), out ZombieMap map))
                _model.Map = map;
        }
    }
}
