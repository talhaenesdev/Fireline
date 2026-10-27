using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using FlowIoC.ScreenModule.Service;
using Modules.LobbyModule.LobbyScreenModule.Models;
using Modules.LobbyModule.LobbyScreenModule.ViewsMediators;
using Modules.PlayerNameModule.Services;

namespace Modules.LobbyModule.LobbyScreenModule.Controllers
{
    /// <summary>The menu's lobby button opens the panel, and pressed again closes it.</summary>
    internal class ToggleLobbyScreenCommand : Command
    {
        [Inject] private IScreenService _screenService { get; set; }
        [Inject] private ILobbyScreenModel _model { get; set; }
        [Inject] private IPlayerNameService _playerNames { get; set; }

        public override async void Execute()
        {
            if (_screenService.Check.IsScreenActive<LobbyScreenView>())
            {
                _screenService.Hide.Screen<LobbyScreenView>();
                return;
            }

            Retain();

            try
            {
                // Read before the await: this instance goes back to the pool when it releases.
                string playerName = _playerNames.Get();
                bool hasState = _model.HasState;
                var state = _model.State;

                LobbyScreenView screen = await _screenService.Open<LobbyScreenView>().Show<LobbyScreenView>();
                if (screen == null)
                {
                    FlowLogger.LogError("ToggleLobbyScreenCommand - the lobby screen did not open.");
                    Stop();
                    return;
                }

                screen.ShowPlayerName(playerName);
                if (hasState)
                    screen.ShowState(state);

                Release();
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"ToggleLobbyScreenCommand threw while opening the screen: {exception}");
                Stop();
            }
        }
    }
}
