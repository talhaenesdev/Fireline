using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.PlayerModule.Entities
{
    /// <summary>A ping on the ground, from the pool: a beam and a ring that pulses until it runs out.</summary>
    public class PingMarker : PoolableItem
    {
        [SerializeField] private Transform _ring;
        [SerializeField] private Transform _beam;

        private float _until;
        private float _shownAt;

        public void Show(Vector3 point, float time)
        {
            transform.position = point;
            _shownAt = Time.time;
            _until = Time.time + time;
        }

        private void Update()
        {
            if (Time.time >= _until)
            {
                Dismiss();
                return;
            }

            float pulse = 1f + 0.25f * Mathf.Sin((Time.time - _shownAt) * 8f);
            _ring.localScale = new Vector3(1.6f * pulse, 0.02f, 1.6f * pulse);
            _beam.localScale = new Vector3(0.12f, 3f * Mathf.Clamp01((Time.time - _shownAt) * 6f), 0.12f);
        }
    }
}
