using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.AudioModule.Services;
using Modules.AudioModule.Shared;
using Modules.WeaponModule.Models;
using UnityEngine;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>The local shot, heard from the muzzle it left.</summary>
    internal class PlayLocalFireSoundCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }
        [Inject] private IAudioService _audio { get; set; }

        public override void Execute() => _audio.PlayAt(AudioKey.Weapon.Fire, _model.ShotPosition);
    }

    internal class PlayReloadSoundCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }
        [Inject] private IAudioService _audio { get; set; }

        public override void Execute()
        {
            if (_model.LocalWeapon != null)
                _audio.PlayAt(AudioKey.Weapon.Reload, _model.LocalWeapon.MuzzlePosition);
        }
    }

    /// <summary>A sound another module or player made, heard from where it happened.</summary>
    internal class PlaySoundAtPayloadCommand : Command<AudioKey>
    {
        [Inject] private IAudioService _audio { get; set; }

        [SignalParam] private Vector3 _position { get; set; }

        public override void Execute(AudioKey key) => _audio.PlayAt(key, _position);
    }
}
