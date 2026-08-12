using System.Collections.Generic;
using UnityEngine;

namespace FireLine.Scripts.Core.Pooling
{
    [CreateAssetMenu(
        fileName = "PoolConfig",
        menuName = "FireLine/Core/Pool Config"
    )]
    public class PoolConfig : ScriptableObject
    {
        [SerializeField]
        private List<PoolData> pools;

        public IReadOnlyList<PoolData> Pools => pools;
    }
}