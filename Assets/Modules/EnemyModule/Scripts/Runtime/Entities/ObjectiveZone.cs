using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>The point to hold, from the pool: a ring the size of the point and a column that rises as it is held.</summary>
    public class ObjectiveZone : PoolableItem
    {
        [SerializeField] private Transform _ring;
        [SerializeField] private Transform _fill;

        public void Show(Vector3 position, float radius, float progress)
        {
            transform.position = new Vector3(position.x, 0f, position.z);
            _ring.localScale = new Vector3(radius * 2f, 0.02f, radius * 2f);
            _fill.localScale = new Vector3(0.5f, Mathf.Max(0.01f, progress * 3f), 0.5f);
            _fill.localPosition = new Vector3(0f, progress * 3f, 0f);
        }
    }
}
