using UnityEngine;

namespace FireLine.Scripts.Network.Signals
{
    public readonly struct NetworkShootSignal
    {
        public readonly Vector3 Position;
        public readonly Vector3 Direction;
        public readonly ulong ClientId;

        public NetworkShootSignal(
            Vector3 position,
            Vector3 direction,
            ulong clientId)
        {
            Position = position;
            Direction = direction;
            ClientId = clientId;
        }
    }
}