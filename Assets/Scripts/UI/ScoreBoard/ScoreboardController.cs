using FireLine.Scripts.Network;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.UI.Scoreboard
{
    public class ScoreboardController : MonoBehaviour
    {
        private NetworkScoreboard _scoreboard;

        [SerializeField]
        private ScoreboardView view;

        [Inject]
        public void Construct(
            NetworkScoreboard scoreboard)
        {
            _scoreboard = scoreboard;

            Debug.Log(
                "[SCOREBOARD CONTROLLER] " +
                "NetworkScoreboard injected."
            );
        }

        private void OnEnable()
        {
            if (_scoreboard == null)
                return;

            _scoreboard.OnScoresChanged +=
                Refresh;

            Refresh();
        }

        private void OnDisable()
        {
            if (_scoreboard == null)
                return;

            _scoreboard.OnScoresChanged -=
                Refresh;
        }

        private void Refresh()
        {
            if (_scoreboard == null)
                return;

            if (view == null)
            {
                Debug.LogError(
                    "[SCOREBOARD CONTROLLER] " +
                    "View is NULL!"
                );

                return;
            }

            view.Clear();

            for (int i = 0;
                 i < _scoreboard.Count;
                 i++)
            {
                NetworkScoreEntry score =
                    _scoreboard.GetAt(i);

                ScoreboardRowView row =
                    view.CreateRow();

                if (row == null)
                    continue;

                row.SetData(
                    $"Player {score.ClientId}",
                    score.Kills,
                    score.Deaths
                );
            }
        }
    }
}