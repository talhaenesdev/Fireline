using UnityEngine;

namespace FireLine.Scripts.Pooling
{
    [CreateAssetMenu(
        fileName = "PoolData",
        menuName = "FireLine/Pooling/Pool Data"
    )]
    public class PoolData : ScriptableObject
    {
        [SerializeField] private string poolKey;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 20;
        [SerializeField] private int maxSize = 100;
        [SerializeField] private bool expandable = true;

        public string PoolKey => poolKey;
        public GameObject Prefab => prefab;
        public int InitialSize => initialSize;
        public int MaxSize => maxSize;
        public bool Expandable => expandable;
    }
}