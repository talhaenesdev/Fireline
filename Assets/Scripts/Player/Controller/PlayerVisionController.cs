using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerVisionController : MonoBehaviour
    {
        [Header("Vision")]
        [SerializeField] private float visionRange = 10f;
        [SerializeField] private float visionAngle = 90f;

        private void Update()
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            // Þimdilik burada hedefleri dýþarýdan vereceðiz.
        }
    }
}