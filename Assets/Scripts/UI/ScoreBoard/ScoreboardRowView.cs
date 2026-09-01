using TMPro;
using UnityEngine;

namespace FireLine.Scripts.UI.Scoreboard
{
    public class ScoreboardRowView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text playerNameText;

        [SerializeField]
        private TMP_Text killsText;

        [SerializeField]
        private TMP_Text deathsText;

        [SerializeField]
        private TMP_Text kdaText;

        public void SetData(
            string playerName,
            int kills,
            int deaths)
        {
            if (playerNameText != null)
                playerNameText.text = playerName;

            if (killsText != null)
                killsText.text = kills.ToString();

            if (deathsText != null)
                deathsText.text = deaths.ToString();

            float kda =
                deaths == 0
                    ? kills
                    : (float)kills / deaths;

            if (kdaText != null)
            {
                kdaText.text =
                    kda.ToString("0.00");
            }
        }
    }
}