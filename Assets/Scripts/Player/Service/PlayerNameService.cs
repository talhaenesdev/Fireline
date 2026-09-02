using UnityEngine;

namespace FireLine.Scripts.Player.Service
{
    public class PlayerNameService
    {
        private const string PlayerNameKey = "PLAYER_NAME";
        private const string DefaultPlayerName = "Player";

        public void SetPlayerName(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = DefaultPlayerName;
            }

            playerName = playerName.Trim();

            if (playerName.Length > 32)
            {
                playerName = playerName.Substring(0, 32);
            }

            PlayerPrefs.SetString(
                PlayerNameKey,
                playerName
            );

            PlayerPrefs.Save();

            Debug.Log(
                $"[PLAYER NAME] Saved | Name={playerName}"
            );
        }

        public string GetPlayerName()
        {
            string playerName =
                PlayerPrefs.GetString(
                    PlayerNameKey,
                    DefaultPlayerName
                );

            if (string.IsNullOrWhiteSpace(playerName))
            {
                return DefaultPlayerName;
            }

            return playerName.Trim();
        }
    }
}