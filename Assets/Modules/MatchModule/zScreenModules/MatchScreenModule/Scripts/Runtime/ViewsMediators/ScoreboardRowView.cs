using TMPro;
using UnityEngine;

namespace Modules.MatchModule.MatchScreenModule.ViewsMediators
{
    /// <summary>One row of the scoreboard, filled by the screen that owns it.</summary>
    public class ScoreboardRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private TMP_Text _killsText;
        [SerializeField] private TMP_Text _deathsText;
        [SerializeField] private TMP_Text _kdaText;

        public void Show(string playerName, int kills, int deaths, float kda)
        {
            if (_playerNameText != null)
                _playerNameText.text = playerName;

            if (_killsText != null)
                _killsText.text = kills.ToString();

            if (_deathsText != null)
                _deathsText.text = deaths.ToString();

            if (_kdaText != null)
                _kdaText.text = kda.ToString("0.00");
        }
    }
}
