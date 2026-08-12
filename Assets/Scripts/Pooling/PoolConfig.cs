using System.Collections.Generic;
using UnityEngine;

namespace FireLine.Scripts.Pooling
{
    [CreateAssetMenu(
        fileName = "PoolConfig",
        menuName = "FireLine/Pooling/Pool Config"
    )]
    public class PoolConfig : ScriptableObject
    {
        [SerializeField]
        private List<PoolData> pools = new();

        public IReadOnlyList<PoolData> Pools => pools;
    }
}