using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.WeaponModule.Entities;
using Modules.WeaponModule.Models;
using Modules.WeaponModule.Signals;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>Every process lets Netcode take the server's rounds from the pool and give them back there.</summary>
    internal class PoolBulletsCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }
        [Inject] private FlowIoC.PoolModule.Services.IPoolService _poolService { get; set; }

        [SignalParam] private IWeapon _weapon { get; set; }

        public override void Execute()
        {
            UnityEngine.GameObject prefab = _weapon.Config != null ? _weapon.Config.NetworkBulletPrefab : null;
            if (prefab == null || Unity.Netcode.NetworkManager.Singleton == null || !_model.PooledBulletPrefabs.Add(prefab))
                return;

            Unity.Netcode.NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new NetworkPoolHandler(_poolService, _weapon.Config.NetworkBulletPoolKey));
        }
    }

    /// <summary>Server: every weapon is kept by its owner, so a bought one can be handed to the right player.</summary>
    internal class TrackServerWeaponCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam] private IWeapon _weapon { get; set; }

        public override void Execute()
        {
            if (_weapon.IsServer)
                _model.ServerWeapons[_weapon.OwnerClientId] = _weapon;
        }
    }

    internal class UntrackServerWeaponCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam] private IWeapon _weapon { get; set; }

        public override void Execute()
        {
            if (_model.ServerWeapons.TryGetValue(_weapon.OwnerClientId, out IWeapon tracked) && tracked == _weapon)
                _model.ServerWeapons.Remove(_weapon.OwnerClientId);
        }
    }

    /// <summary>Server: the buyer now carries the weapon they bought.</summary>
    internal class EquipWeaponCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private int _index { get; set; }

        public override void Execute()
        {
            if (_model.ServerWeapons.TryGetValue(_clientId, out IWeapon weapon))
                weapon.SetWeaponServer(_index);
        }
    }

    /// <summary>The local player's new weapon comes loaded; anybody else's swap ends the flow here.</summary>
    internal class ApplyLocalWeaponChangeCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam] private IWeapon _weapon { get; set; }

        public override void Execute()
        {
            if (_model.LocalWeapon != _weapon || _weapon.Config == null)
            {
                Retain();
                Stop();
                return;
            }

            _model.Ammo = _weapon.Config.MagazineSize;
            _model.IsReloading = false;
            _model.NextFireTime = 0f;
        }
    }

    /// <summary>The HUD names the weapon the local player carries.</summary>
    internal class PublishLocalWeaponCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }
        [InjectSignal] private WeaponSignals _signals { get; set; }

        public override void Execute()
        {
            if (_model.LocalWeapon?.Config != null)
                _signals.Outgoing.LocalWeaponChanged.Dispatch(_model.LocalWeapon.Config.DisplayName);
        }
    }
}
