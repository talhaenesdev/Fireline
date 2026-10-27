using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using Modules.WeaponModule.Data.UnityObjects;
using Modules.WeaponModule.Entities;
using Unity.Netcode;
using UnityEngine;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>The server fires the round that counts, owned by the shooter so the shooter can hide it.</summary>
    internal class SpawnServerBulletCommand : Command
    {
        [SignalParam] private IWeapon _shooter { get; set; }
        [SignalParam(0)] private Vector3 _position { get; set; }
        [SignalParam(1)] private Vector3 _direction { get; set; }

        public override void Execute()
        {
            CD_Weapon config = _shooter.Config;

            if (!_shooter.IsServer || config == null || _direction == Vector3.zero)
            {
                Retain();
                Stop();
                return;
            }

            if (config.NetworkBulletPrefab == null)
            {
                FlowLogger.LogError($"SpawnServerBulletCommand - '{config.name}' has no network bullet prefab.");
                Retain();
                Stop();
                return;
            }

            Vector3 direction = _direction.normalized;
            GameObject instance = Object.Instantiate(config.NetworkBulletPrefab, _position, Quaternion.LookRotation(direction));

            instance.GetComponent<NetworkObject>().SpawnWithOwnership(_shooter.OwnerClientId);
            instance.GetComponent<INetworkBullet>().Launch(direction, config.BulletSpeed, config.BulletLifetime,
                config.Damage, _shooter.OwnerClientId);
        }
    }
}
