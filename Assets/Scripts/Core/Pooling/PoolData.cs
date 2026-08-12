using UnityEngine;

namespace FireLine.Scripts.Core.Pooling
{
    [CreateAssetMenu(
        fileName = "PoolData",
        menuName = "FireLine/Core/Pool Data"
    )]
    public class PoolData : ScriptableObject
    {
        [SerializeField] private string poolId;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 20;
        [SerializeField] private int maxSize = 100;
        [SerializeField] private bool expandable = true;

        public string PoolId => poolId;
        public GameObject Prefab => prefab;
        public int InitialSize => initialSize;
        public int MaxSize => maxSize;
        public bool Expandable => expandable;
    }
}