using FireLine.Scripts.Player.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerNameInputController : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inputField;

        private PlayerNameService _playerNameService;

        [Inject]
        public void Construct(
            PlayerNameService playerNameService)
        {
            _playerNameService =
                playerNameService;

            Debug.Log(
                "[PLAYER NAME INPUT] " +
                "PlayerNameService injected."
            );
        }

        private void Start()
        {
            if (inputField == null)
            {
                Debug.LogError(
                    "[PLAYER NAME INPUT] " +
                    "InputField is NULL!"
                );

                return;
            }

            if (_playerNameService == null)
            {
                Debug.LogError(
                    "[PLAYER NAME INPUT] " +
                    "PlayerNameService is NULL!"
                );

                return;
            }

            inputField.text =
                _playerNameService.GetPlayerName();

            inputField.onEndEdit.AddListener(
                OnNameChanged
            );
        }

        private void OnNameChanged(
            string playerName)
        {
            if (_playerNameService == null)
                return;

            _playerNameService.SetPlayerName(
                playerName
            );
        }

        public void SaveName()
        {
            if (inputField == null)
                return;

            if (_playerNameService == null)
                return;

            _playerNameService.SetPlayerName(
                inputField.text
            );
        }

        private void OnDestroy()
        {
            if (inputField != null)
            {
                inputField.onEndEdit.RemoveListener(
                    OnNameChanged
                );
            }
        }
    }
}