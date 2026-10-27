using FlowIoC.BaseModule.Signals;
using Modules.WeaponModule.Entities;
using UnityEngine;

namespace Modules.WeaponModule.Signals
{
    internal class WeaponInternalSignals : ISignalHolder
    {
        public Signal Tick = new(hideCommandLog: true);

        public Signal<IWeapon> WeaponSpawned = new();
        public Signal<IWeapon> WeaponDespawned = new();

        public Signal Reload = new();

        /// <summary>The local weapon fired: show the flash and play the animation.</summary>
        public Signal ShowLocalFire = new();

        /// <summary>The shooter's weapon, then where and which way it fired. Server only.</summary>
        public Signal<IWeapon, Vector3, Vector3> ServerFireRequested = new();

        /// <summary>Another player fired here.</summary>
        public Signal<Vector3> RemoteFired = new();

        public Signal<INetworkBullet, Collider> BulletHit = new();

        /// <summary>A round struck a wall here.</summary>
        public Signal<Vector3> ImpactShown = new();
    }
}
