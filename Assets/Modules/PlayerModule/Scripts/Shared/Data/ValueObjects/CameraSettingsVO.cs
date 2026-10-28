namespace Modules.PlayerModule.Shared.Data.ValueObjects
{
    /// <summary>
    /// The local player's camera as it is set, and how far each setting may go. The ranges come from
    /// the CD_CameraSettings asset, so a settings panel offers exactly what the camera accepts.
    /// </summary>
    public readonly struct CameraSettingsVO
    {
        public readonly float Height;
        public readonly float HeightMin;
        public readonly float HeightMax;
        public readonly float FollowSpeed;
        public readonly float FollowSpeedMin;
        public readonly float FollowSpeedMax;
        public readonly float Shake;
        public readonly float ShakeMax;

        public CameraSettingsVO(float height, float heightMin, float heightMax,
                                float followSpeed, float followSpeedMin, float followSpeedMax,
                                float shake, float shakeMax)
        {
            Height = height;
            HeightMin = heightMin;
            HeightMax = heightMax;
            FollowSpeed = followSpeed;
            FollowSpeedMin = followSpeedMin;
            FollowSpeedMax = followSpeedMax;
            Shake = shake;
            ShakeMax = shakeMax;
        }
    }
}
