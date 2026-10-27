using UnityEngine;

namespace Modules.PlayerModule.Entities
{
    /// <summary>
    /// One networked player as the module's commands handle it. Its state is replicated by
    /// Netcode, so the server writes it here rather than in a Model; the setters do nothing on a
    /// client.
    /// </summary>
    internal interface IPlayerAvatar
    {
        ulong ClientId { get; }
        bool IsOwner { get; }
        bool IsServer { get; }
        Transform Transform { get; }

        string PlayerName { get; }
        float MoveSpeed { get; }
        float SprintSpeed { get; }
        float Health { get; }
        float MaxHealth { get; }
        bool IsDead { get; }

        /// <summary>Owner only: asks the server to put this name on the avatar.</summary>
        void RequestName(string playerName);

        void SetNameServer(string playerName);
        void SetHealthServer(float health);
        void SetDeadServer(bool isDead);
        void PlayHitEffectServer();
    }
}
