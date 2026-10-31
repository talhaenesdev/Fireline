using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>
    /// A gob of spit as each client sees it. The server flies the real one and decides the hit; this
    /// one flies the same line for as long as the server's would, and goes back to its pool.
    /// </summary>
    public class SpitProjectile : PoolableItem
    {
        private Vector3 _velocity;
        private float _timeLeft;

        public void Launch(Vector3 position, Vector3 velocity, float lifetime)
        {
            transform.position = position;
            _velocity = velocity;
            _timeLeft = lifetime;
        }

        private void Update()
        {
            transform.position += _velocity * Time.deltaTime;
            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0f)
                Dismiss();
        }
    }
}
