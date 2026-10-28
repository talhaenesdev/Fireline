using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Data.UnityObjects;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Shared.Data.ValueObjects;
using Modules.PlayerModule.Signals;
using UnityEngine;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>
    /// The local camera came up with its config: the player's saved choices, or the config's
    /// defaults where there are none, kept inside the config's ranges.
    /// </summary>
    internal class LoadCameraSettingsCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        [SignalParam] private CD_CameraSettings _config { get; set; }

        public override void Execute()
        {
            _model.CameraConfig = _config;
            _model.CameraHeight = Mathf.Clamp(PlayerPrefs.GetFloat(CameraSettingsKeys.HEIGHT, _config.Height), _config.HeightMin, _config.HeightMax);
            _model.CameraFollowSpeed = Mathf.Clamp(PlayerPrefs.GetFloat(CameraSettingsKeys.FOLLOW_SPEED, _config.FollowSpeed), _config.FollowSpeedMin, _config.FollowSpeedMax);
            _model.CameraShake = Mathf.Clamp(PlayerPrefs.GetFloat(CameraSettingsKeys.SHAKE, _config.Shake), 0f, _config.ShakeMax);
        }
    }

    /// <summary>A player moved a setting; it is taken within the config's ranges. Before the camera is up there is nothing to set.</summary>
    internal class StoreCameraSettingsCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        [SignalParam(0)] private float _height { get; set; }
        [SignalParam(1)] private float _followSpeed { get; set; }
        [SignalParam(2)] private float _shake { get; set; }

        public override void Execute()
        {
            CD_CameraSettings config = _model.CameraConfig;
            if (config == null)
            {
                Retain();
                Stop();
                return;
            }

            _model.CameraHeight = Mathf.Clamp(_height, config.HeightMin, config.HeightMax);
            _model.CameraFollowSpeed = Mathf.Clamp(_followSpeed, config.FollowSpeedMin, config.FollowSpeedMax);
            _model.CameraShake = Mathf.Clamp(_shake, 0f, config.ShakeMax);
        }
    }

    /// <summary>The choices are the player's, so they stay on this device for the next match.</summary>
    internal class SaveCameraSettingsCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        public override void Execute()
        {
            PlayerPrefs.SetFloat(CameraSettingsKeys.HEIGHT, _model.CameraHeight);
            PlayerPrefs.SetFloat(CameraSettingsKeys.FOLLOW_SPEED, _model.CameraFollowSpeed);
            PlayerPrefs.SetFloat(CameraSettingsKeys.SHAKE, _model.CameraShake);
            PlayerPrefs.Save();
        }
    }

    /// <summary>The camera takes the settings, and whoever shows them is told.</summary>
    internal class PublishCameraSettingsCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }
        [InjectSignal] private PlayerInternalSignals _internalSignals { get; set; }

        public override void Execute()
        {
            CD_CameraSettings config = _model.CameraConfig;
            _internalSignals.ApplyLocalCamera.Dispatch(_model.CameraHeight, _model.CameraFollowSpeed, _model.CameraShake);
            _signals.Outgoing.CameraSettingsChanged.Dispatch(new CameraSettingsVO(
                _model.CameraHeight, config.HeightMin, config.HeightMax,
                _model.CameraFollowSpeed, config.FollowSpeedMin, config.FollowSpeedMax,
                _model.CameraShake, config.ShakeMax));
        }
    }

    internal static class CameraSettingsKeys
    {
        public const string HEIGHT = "CAMERA_HEIGHT";
        public const string FOLLOW_SPEED = "CAMERA_FOLLOW_SPEED";
        public const string SHAKE = "CAMERA_SHAKE";
    }
}
