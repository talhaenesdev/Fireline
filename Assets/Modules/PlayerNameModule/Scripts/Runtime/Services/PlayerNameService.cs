using UnityEngine;

namespace Modules.PlayerNameModule.Services
{
    internal class PlayerNameService : IPlayerNameService
    {
        // The key the Zenject version stored under, so a name typed before the migration survives it.
        private const string PLAYER_NAME_KEY = "PLAYER_NAME";
        private const string DEFAULT_PLAYER_NAME = "Player";
        private const int MAX_LENGTH = 32;

        public string Get() => Clean(PlayerPrefs.GetString(PLAYER_NAME_KEY, DEFAULT_PLAYER_NAME));

        public string Set(string playerName)
        {
            string cleaned = Clean(playerName);
            PlayerPrefs.SetString(PLAYER_NAME_KEY, cleaned);
            PlayerPrefs.Save();
            return cleaned;
        }

        public string Clean(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
                return DEFAULT_PLAYER_NAME;

            string trimmed = playerName.Trim();
            return trimmed.Length > MAX_LENGTH ? trimmed.Substring(0, MAX_LENGTH) : trimmed;
        }
    }
}
