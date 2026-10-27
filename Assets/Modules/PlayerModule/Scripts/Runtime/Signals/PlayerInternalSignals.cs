using FlowIoC.BaseModule.Signals;
using Modules.PlayerModule.Entities;
using UnityEngine;

namespace Modules.PlayerModule.Signals
{
    /// <summary>What the module says to its own commands and mediators.</summary>
    internal class PlayerInternalSignals : ISignalHolder
    {
        public Signal Tick = new(hideCommandLog: true);

        public Signal<IPlayerAvatar> AvatarSpawned = new();
        public Signal<IPlayerAvatar> AvatarDespawned = new();

        /// <summary>Client id, then the name it asked for. Server only.</summary>
        public Signal<ulong, string> NameRequested = new();

        /// <summary>Client id, then the name now on its avatar.</summary>
        public Signal<ulong, string> NameChanged = new();

        /// <summary>Client id, current health, maximum health.</summary>
        public Signal<ulong, float, float> HealthChanged = new();

        /// <summary>Client id, dead.</summary>
        public Signal<ulong, bool> DeathChanged = new();

        public Signal<Vector2> MoveInput = new();
        public Signal<bool> SprintInput = new();

        /// <summary>The step the local avatar takes this frame, its input and whether it sprints.</summary>
        public Signal<Vector3, Vector2, bool> MoveLocalAvatar = new(hideCommandLog: true);

        public Signal ShakeLocalCamera = new();

        /// <summary>The way the local avatar is turned when it aims by itself.</summary>
        public Signal<Vector3> FaceLocalAvatar = new(hideCommandLog: true);
    }
}
