using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>A ring of force bursting out to its radius and thinning away: an explosion or a slam.</summary>
    public class BlastRing : PoolableItem
    {
        [SerializeField] private Renderer _ring;
        [SerializeField] private float _lifetime = 0.4f;

        private readonly int _baseColorId = Shader.PropertyToID("_BaseColor");
        private MaterialPropertyBlock _block;
        private float _radius;
        private float _age;

        public void Show(Vector3 position, float radius, Color color)
        {
            transform.position = new Vector3(position.x, 0.05f, position.z);
            _radius = radius;
            _age = 0f;

            _block ??= new MaterialPropertyBlock();
            _block.SetColor(_baseColorId, color);
            _ring.SetPropertyBlock(_block);
            Draw();
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_age >= _lifetime)
            {
                Dismiss();
                return;
            }

            Draw();
        }

        private void Draw()
        {
            float t = _age / _lifetime;
            float size = _radius * 2f * Mathf.Lerp(0.2f, 1f, 1f - (1f - t) * (1f - t));
            transform.localScale = new Vector3(size, Mathf.Lerp(0.6f, 0.02f, t), size);
        }
    }
}
