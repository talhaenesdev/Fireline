using UnityEngine;

namespace Modules.WeaponModule.Entities
{
    /// <summary>A round the server simulates; clients see it through Netcode.</summary>
    internal interface INetworkBullet
    {
        ulong ShooterClientId { get; }
        int Damage { get; }
        Vector3 Position { get; }

        void Launch(Vector3 direction, float speed, float lifetime, int damage, ulong shooterClientId);

        /// <summary>Server only: tells every client a round struck something here.</summary>
        void ShowImpact(Vector3 point);

        void Despawn();
    }
}
