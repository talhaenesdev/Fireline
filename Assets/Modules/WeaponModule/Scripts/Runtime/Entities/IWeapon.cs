using Modules.WeaponModule.Data.UnityObjects;
using UnityEngine;

namespace Modules.WeaponModule.Entities
{
    /// <summary>The weapon a networked player carries, as the module's commands handle it.</summary>
    internal interface IWeapon
    {
        ulong OwnerClientId { get; }
        bool IsOwner { get; }
        bool IsServer { get; }
        CD_Weapon Config { get; }

        /// <summary>Which weapon of the set it is.</summary>
        int WeaponIndex { get; }

        /// <summary>Server only: the player now carries this weapon of the set.</summary>
        void SetWeaponServer(int index);
        Transform Owner { get; }
        Vector3 MuzzlePosition { get; }

        /// <summary>Owner only: asks the server to fire a round from here, this way.</summary>
        void RequestServerFire(Vector3 position, Vector3 direction);

        /// <summary>Server only: lets every client but the shooter hear the shot.</summary>
        void BroadcastFire(Vector3 position);
    }
}
