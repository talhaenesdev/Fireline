using UnityEngine;

namespace Modules.PlayerModule.Data.UnityObjects
{
    /// <summary>
    /// Where the local player's camera starts, and how far the settings panel lets a player move it.
    /// What a player chooses is saved on their device and wins over the defaults here.
    /// </summary>
    [CreateAssetMenu(fileName = "CD_CameraSettings", menuName = "Fireline/Player/CD_CameraSettings")]
    public class CD_CameraSettings : ScriptableObject
    {
        [Header("Height: the orthographic size of a top-down camera, or its height above the player")]
        [SerializeField] private float _height = 13f;
        [SerializeField] private float _heightMin = 8f;
        [SerializeField] private float _heightMax = 25f;

        [Header("How quickly the camera catches up with the player")]
        [SerializeField] private float _followSpeed = 10f;
        [SerializeField] private float _followSpeedMin = 3f;
        [SerializeField] private float _followSpeedMax = 20f;

        [Header("How hard the camera shakes when firing, times CD_CameraShake")]
        [SerializeField] private float _shake = 1f;
        [SerializeField] private float _shakeMax = 2f;

        public float Height => _height;
        public float HeightMin => _heightMin;
        public float HeightMax => _heightMax;
        public float FollowSpeed => _followSpeed;
        public float FollowSpeedMin => _followSpeedMin;
        public float FollowSpeedMax => _followSpeedMax;
        public float Shake => _shake;
        public float ShakeMax => _shakeMax;
    }
}
