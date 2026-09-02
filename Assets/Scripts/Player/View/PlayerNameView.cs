using TMPro;
using UnityEngine;

namespace FireLine.Scripts.Player.View
{
    public class PlayerNameView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text playerNameText;

        public void SetPlayerName(string playerName)
        {
            if (playerNameText == null)
            {
                Debug.LogError(
                    "[PLAYER NAME VIEW] " +
                    "Player name text is NULL!"
                );

                return;
            }

            playerNameText.text =
                string.IsNullOrWhiteSpace(playerName)
                    ? "Player"
                    : playerName;
        }
    }
}