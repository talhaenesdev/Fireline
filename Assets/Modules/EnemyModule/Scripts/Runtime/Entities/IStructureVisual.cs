using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>A structure as each client draws it, from its pool.</summary>
    public interface IStructureVisual : IPoolableItem
    {
        void Place(Vector3 position, float scale, bool showsHealth);

        void SetState(float yaw, float healthShare, Color color);

        void ShowShot(Vector3 target);
    }
}
