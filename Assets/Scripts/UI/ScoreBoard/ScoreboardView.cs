using TMPro;
using UnityEngine;

namespace FireLine.Scripts.UI.Scoreboard
{
    public class ScoreboardView : MonoBehaviour
    {
        [SerializeField]
        private Transform content;

        [SerializeField]
        private ScoreboardRowView rowPrefab;

        public void Clear()
        {
            if (content == null)
                return;

            for (int i = content.childCount - 1; i >= 0; i--)
            {
                Destroy(
                    content.GetChild(i).gameObject
                );
            }
        }

        public ScoreboardRowView CreateRow()
        {
            if (content == null)
            {
                Debug.LogError(
                    "[SCOREBOARD VIEW] Content is NULL!"
                );

                return null;
            }

            if (rowPrefab == null)
            {
                Debug.LogError(
                    "[SCOREBOARD VIEW] Row prefab is NULL!"
                );

                return null;
            }

            return Instantiate(
                rowPrefab,
                content
            );
        }
    }
}