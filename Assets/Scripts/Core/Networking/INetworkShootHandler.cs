using UnityEngine;

namespace FireLine.Scripts.Core.Networking
{
    public interface INetworkShootHandler
    {
        void Shoot(
            Vector3 position,
            Vector3 direction);
    }
}