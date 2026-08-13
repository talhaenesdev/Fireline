using FireLine.Scripts.Enemy.Data;
using FireLine.Scripts.Enemy.Factory;
using UnityEngine;
using Zenject;

public class EnemySpawnTest : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;

    [Inject]
    private IEnemyFactory _enemyFactory;

    private void Start()
    {
        _enemyFactory.Create(
            enemyData,
            transform.position,
            Quaternion.identity
        );
    }
}