using FlowIoC.BaseModule.Signals;

namespace Modules.EnemyModule.Signals
{
    public class EnemySignals : ISignalHolder
    {
        public EnemySignalsIncoming Incoming = new();
        public EnemySignalsOutgoing Outgoing = new();
    }

    public class EnemySignalsIncoming
    {
        /// <summary>The match is a zombie match: begin the waves. Server only.</summary>
        public Signal StartWaves = new();

        /// <summary>A networked object was hit: its NetworkObjectId, damage, shooting client. Server only.</summary>
        public Signal<ulong, int, ulong> DamageZombie = new();
    }

    public class EnemySignalsOutgoing
    {
        /// <summary>A zombie reached a player: target client, damage, attacker id. Server only.</summary>
        public Signal<ulong, int, ulong> PlayerAttacked = new();

        /// <summary>A zombie went down to the shooting client. Server only.</summary>
        public Signal<ulong> ZombieKilled = new();
    }
}
