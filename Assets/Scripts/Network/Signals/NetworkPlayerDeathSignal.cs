namespace FireLine.Scripts.Network.Signals
{
    public class NetworkPlayerDeathSignal
    {
        public ulong ClientId { get; }

        public NetworkPlayerDeathSignal(ulong clientId)
        {
            ClientId = clientId;
        }
    }
}