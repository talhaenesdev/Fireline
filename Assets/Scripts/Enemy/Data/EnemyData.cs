using UnityEngine;

namespace FireLine.Scripts.Enemy.Data
{
    [CreateAssetMenu(
        fileName = "EnemyData",
        menuName = "FireLine/Enemy/Enemy Data"
    )]
    public class EnemyData : ScriptableObject
    {
        [SerializeField]
        private string poolKey;

        public string PoolKey => poolKey;
    }
}