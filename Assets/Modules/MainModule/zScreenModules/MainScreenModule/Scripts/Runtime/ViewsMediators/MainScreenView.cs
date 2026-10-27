using System;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.MainModule.MainScreenModule.ViewsMediators
{
    /// <summary>Fireline's main menu: the background, the logo, the lobby button and quit.</summary>
    [RequireComponent(typeof(ViewInjector))]
    public class MainScreenView : ScreenView
    {
        [SerializeField] private Button _lobbyButton;
        [SerializeField] private Button _quitButton;

        public Action LobbyClicked;
        public Action QuitClicked;

        private void OnEnable()
        {
            _lobbyButton.onClick.AddListener(OnLobbyClicked);
            _quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnDisable()
        {
            _lobbyButton.onClick.RemoveListener(OnLobbyClicked);
            _quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnLobbyClicked() => LobbyClicked?.Invoke();
        private void OnQuitClicked() => QuitClicked?.Invoke();

        protected override void PlayShowAnimation() => ShowCompleted?.Invoke(this);

        protected override void PlayHideAnimation() => HideCompleted?.Invoke(this);
    }
}
