using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>A barricade as each client draws it, from its pool: planks and a health bar.</summary>
    public class BarricadeVisual : PoolableItem, IStructureVisual
    {
        [SerializeField] private Transform _healthBar;
        [SerializeField] private Transform _healthFill;

        public override void OnGetFromPool()
        {
            transform.localScale = Vector3.one;
            _healthBar.gameObject.SetActive(true);
            SetHealth(1f);
        }

        public void Place(Vector3 position, float scale, bool showsHealth)
        {
            transform.position = position;
            transform.localScale = Vector3.one * scale;
            _healthBar.gameObject.SetActive(showsHealth);
        }

        public void SetState(float yaw, float healthShare, Color color)
        {
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            SetHealth(healthShare);
        }

        public void ShowShot(Vector3 target) { }

        private void SetHealth(float share)
        {
            _healthFill.localScale = new Vector3(share, 1f, 1f);
            _healthFill.localPosition = new Vector3((share - 1f) * 0.5f, 0f, 0f);
        }
    }
}
