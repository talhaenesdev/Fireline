using UnityEngine;

namespace Modules.PlayerModule.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "CD_CameraShake", menuName = "Fireline/Player/CD_CameraShake")]
    public class CD_CameraShake : ScriptableObject
    {
        [SerializeField] private float _duration = 0.06f;
        [SerializeField] private float _strength = 0.04f;
        [SerializeField] private float _frequency = 25f;

        public float Duration => _duration;
        public float Strength => _strength;
        public float Frequency => _frequency;
    }
}
