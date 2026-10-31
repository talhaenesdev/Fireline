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

        /// <summary>How long a revive this player makes takes, against the usual; a perk brings it down.</summary>
        float ReviveScale { get; }

        /// <summary>How fast this player's feet are, against the usual; a perk brings it up.</summary>
        float SpeedScale { get; }

        /// <summary>Server only: perks.</summary>
        void RaiseMaxHealthServer(float share);
        void ScaleReviveServer(float share);
        void ScaleSpeedServer(float share);

        /// <summary>Owner only: asks the server to ping this point for everyone.</summary>
        void RequestPing(Vector3 point);

        /// <summary>Server only: every client shows this player's ping.</summary>
        void ShowPingServer(Vector3 point);
        bool IsDead { get; }
        float Shield { get; }

        /// <summary>Owner only: asks the server to put this name on the avatar.</summary>
        void RequestName(string playerName);

        void SetNameServer(string playerName);
        void SetHealthServer(float health);
        void SetDeadServer(bool isDead);
        void SetShieldServer(float shield);

        /// <summary>Owner only: asks the server to start reviving the player with this client id.</summary>
        void RequestRevive(ulong targetClientId);
        void PlayHitEffectServer();
    }
}
