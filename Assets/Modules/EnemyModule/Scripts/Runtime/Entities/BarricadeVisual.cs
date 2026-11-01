using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>A barricade as each client draws it, from its pool: planks and a health bar.</summary>
    public class BarricadeVisual : PoolableItem, IStructureVisual
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] private Transform _healthBar;
        [SerializeField] private Transform _healthFill;
        [Tooltip("Keeps players out once the structure stands; off while it is carried. Optional.")]
        [SerializeField] private Collider _blocker;
        [Tooltip("Cuts the structure out of the NavMesh while it stands, so zombies walk round it. Optional.")]
        [SerializeField] private Behaviour _obstacle;

        private MaterialPropertyBlock _block;

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
            if (_blocker != null)
                _blocker.enabled = showsHealth;
            if (_obstacle != null)
                _obstacle.enabled = showsHealth;
        }

        public void SetState(float yaw, float healthShare, Color color)
        {
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            SetHealth(healthShare);
            Tint(color);
        }

        // A clear colour takes the tint off; any other dresses every piece but the health bar in it.
        private void Tint(Color color)
        {
            _block ??= new MaterialPropertyBlock();
            foreach (Renderer piece in GetComponentsInChildren<Renderer>(true))
            {
                if (piece.transform.IsChildOf(_healthBar))
                    continue;

                if (color.a <= 0f)
                {
                    piece.SetPropertyBlock(null);
                    continue;
                }

                _block.SetColor(BaseColorId, color);
                piece.SetPropertyBlock(_block);
            }
        }

        public void ShowShot(Vector3 target) { }

        private void SetHealth(float share)
        {
            _healthFill.localScale = new Vector3(share, 1f, 1f);
            _healthFill.localPosition = new Vector3((share - 1f) * 0.5f, 0f, 0f);
        }
    }
}
