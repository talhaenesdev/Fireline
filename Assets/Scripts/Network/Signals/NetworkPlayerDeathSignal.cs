namespace FireLine.Scripts.Network.Signals
{
    public class NetworkPlayerDeathSignal
    {
        public ulong VictimClientId { get; }
        public ulong KillerClientId { get; }

        public NetworkPlayerDeathSignal(
            ulong victimClientId,
            ulong killerClientId)
        {
            VictimClientId = victimClientId;
            KillerClientId = killerClientId;
        }
    }
}