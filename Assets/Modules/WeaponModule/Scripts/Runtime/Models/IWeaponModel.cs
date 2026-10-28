using Modules.WeaponModule.Entities;
using UnityEngine;

namespace Modules.WeaponModule.Models
{
    /// <summary>The local player's weapon and the state of its magazine and trigger.</summary>
    internal interface IWeaponModel
    {
        IWeapon LocalWeapon { get; set; }

        bool IsArmed { get; set; }
        int Ammo { get; set; }
        bool IsReloading { get; set; }
        float NextFireTime { get; set; }

        bool IsTriggerHeld { get; set; }
        bool WasTriggerPressed { get; set; }
        /// <summary>The point on the ground being aimed at, and whether there is one yet.</summary>
        Vector3 AimPoint { get; set; }
        bool HasAim { get; set; }

        /// <summary>Multiplies the weapon's time between shots; bought upgrades bring it down.</summary>
        float FireRateMultiplier { get; set; }

        /// <summary>Where and which way the round just fired went, for the steps after the shot.</summary>
        Vector3 ShotPosition { get; set; }
        Vector3 ShotDirection { get; set; }

        bool IsTicking { get; set; }
        bool IsHooked { get; set; }
    }

    internal class WeaponModel : IWeaponModel
    {
        public IWeapon LocalWeapon { get; set; }
        public bool IsArmed { get; set; }
        public int Ammo { get; set; }
        public bool IsReloading { get; set; }
        public float NextFireTime { get; set; }
        public bool IsTriggerHeld { get; set; }
        public bool WasTriggerPressed { get; set; }
        public Vector3 AimPoint { get; set; }
        public bool HasAim { get; set; }
        public float FireRateMultiplier { get; set; } = 1f;
        public Vector3 ShotPosition { get; set; }
        public Vector3 ShotDirection { get; set; }
        public bool IsTicking { get; set; }
        public bool IsHooked { get; set; }
    }
}
