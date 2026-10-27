using System;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using Modules.LobbyModule.Shared.Data.ValueObjects;
using Modules.MatchModule.Shared.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.LobbyModule.LobbyScreenModule.ViewsMediators
{
    [RequireComponent(typeof(ViewInjector))]
    public class LobbyScreenView : ScreenView
    {
        [Header("Buttons")]
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _startGameButton;

        [Header("Input")]
        [SerializeField] private TMP_InputField _joinCodeInput;
        [SerializeField] private TMP_InputField _playerNameInput;

        [Header("Texts")]
        [SerializeField] private TMP_Text _sessionCodeText;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private TMP_Text _hostStatusText;
        [SerializeField] private TMP_Text _playerCountText;
        [SerializeField] private GameObject _readyMark;

        public Action<string> CreateClicked;
        public Action<string, string> JoinClicked;
        public Action LeaveClicked;
        public Action ReadyClicked;
        public Action StartClicked;
        public Action ModeClicked;

        // Built at runtime on the panel's right edge, so the prefab needs no new objects.
        private Button _modeButton;
        private TMP_Text _modeText;

        private void Awake() => BuildModeButton();

        private void OnEnable()
        {
            _createButton.onClick.AddListener(OnCreateClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
            _leaveButton.onClick.AddListener(OnLeaveClicked);
            _readyButton.onClick.AddListener(OnReadyClicked);
            _startGameButton.onClick.AddListener(OnStartClicked);
            _modeButton.onClick.AddListener(OnModeClicked);
        }

        private void OnDisable()
        {
            _createButton.onClick.RemoveListener(OnCreateClicked);
            _joinButton.onClick.RemoveListener(OnJoinClicked);
            _leaveButton.onClick.RemoveListener(OnLeaveClicked);
            _readyButton.onClick.RemoveListener(OnReadyClicked);
            _startGameButton.onClick.RemoveListener(OnStartClicked);
            _modeButton.onClick.RemoveListener(OnModeClicked);
        }

        public void ShowPlayerName(string playerName) => _playerNameInput.text = playerName;

        public void ShowState(LobbyStateVO state)
        {
            _readyButton.interactable = !state.IsBusy && state.HasSession;
            _createButton.interactable = !state.IsBusy && !state.HasSession;
            _joinButton.interactable = !state.IsBusy && !state.HasSession;

            _leaveButton.gameObject.SetActive(state.HasSession);
            _leaveButton.interactable = !state.IsBusy;

            _startGameButton.gameObject.SetActive(state.IsHost);
            _startGameButton.interactable = state.CanStart;

            _sessionCodeText.text = state.HasSession ? $"JOIN CODE: {state.JoinCode}" : "JOIN CODE: -";
            _hostStatusText.text = !state.HasSession ? "NOT CONNECTED" : state.IsHost ? "YOU ARE HOST" : "YOU ARE CLIENT";
            _playerCountText.text = $"PLAYERS: {state.PlayerCount} / {state.MaxPlayers}";
            _statusText.text = state.Status;

            _modeText.text = state.Mode == GameMode.Zombie ? "MODE: ZOMBIE" : "MODE: DEATHMATCH";
            // Chosen freely before a lobby exists; inside one, only the host changes it.
            _modeButton.interactable = !state.IsBusy && (!state.HasSession || state.IsHost);

            if (_readyMark != null)
                _readyMark.SetActive(state.IsLocalReady);

            // The host's code is filled in for them. A client's field is never touched: a state that
            // arrives while the code is being typed would otherwise wipe it.
            if (state.HasSession && state.IsHost)
                _joinCodeInput.text = state.JoinCode;
        }

        private void OnCreateClicked() => CreateClicked?.Invoke(_playerNameInput.text);
        private void OnJoinClicked() => JoinClicked?.Invoke(_playerNameInput.text, _joinCodeInput.text);
        private void OnLeaveClicked() => LeaveClicked?.Invoke();
        private void OnReadyClicked() => ReadyClicked?.Invoke();
        private void OnStartClicked() => StartClicked?.Invoke();
        private void OnModeClicked() => ModeClicked?.Invoke();

        private void BuildModeButton()
        {
            // On the right edge of the lobby panel, centred top to bottom.
            var panel = (RectTransform) transform.GetChild(0);
            var go = new GameObject("ModeButton", typeof(RectTransform));
            var rect = (RectTransform) go.transform;
            rect.SetParent(panel, false);
            rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-20f, 0f);
            rect.sizeDelta = new Vector2(240f, 44f);

            go.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.18f, 0.9f);
            _modeButton = go.AddComponent<Button>();

            var label = new GameObject("Label", typeof(RectTransform));
            var labelRect = (RectTransform) label.transform;
            labelRect.SetParent(rect, false);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;

            _modeText = label.AddComponent<TextMeshProUGUI>();
            _modeText.fontSize = 22f;
            _modeText.alignment = TextAlignmentOptions.Center;
            _modeText.raycastTarget = false;
            _modeText.text = "MODE: DEATHMATCH";
        }

        protected override void PlayShowAnimation() => ShowCompleted?.Invoke(this);

        protected override void PlayHideAnimation() => HideCompleted?.Invoke(this);
    }
}
