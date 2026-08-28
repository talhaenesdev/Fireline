namespace FireLine.Scripts.Network.Model
{
    public class NetworkPlayerScore
    {
        public ulong ClientId { get; }

        public int Kills { get; private set; }
        public int Deaths { get; private set; }

        public NetworkPlayerScore(
            ulong clientId)
        {
            ClientId = clientId;
        }

        public void AddKill()
        {
            Kills++;
        }

        public void AddDeath()
        {
            Deaths++;
        }
    }
}