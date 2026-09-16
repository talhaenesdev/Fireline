using UnityEngine;

namespace FireLine.Scripts.Player.Model
{
    [CreateAssetMenu(
        fileName = "CameraShakeData",
        menuName = "FireLine/Player/Camera Shake Data"
    )]
    public class CameraShakeData : ScriptableObject
    {
        [Header("Position")]
        [SerializeField]
        private float duration = 0.06f;

        [SerializeField]
        private float strength = 0.05f;

        [SerializeField]
        private float frequency = 25f;

        public float Duration => duration;
        public float Strength => strength;
        public float Frequency => frequency;
    }
}