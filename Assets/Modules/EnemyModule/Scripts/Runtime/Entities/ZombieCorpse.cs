using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>
    /// A fallen zombie, from the corpse pool: it topples over in its kind's colour and size, lies in its
    /// blood for a moment, then sinks into the ground and goes back to the pool.
    /// </summary>
    public class ZombieCorpse : PoolableItem
    {
        [SerializeField] private Transform _body;
        [SerializeField] private Renderer _bodyRenderer;
        [SerializeField] private Transform _blood;
        [SerializeField] private float _fallTime = 0.35f;
        [SerializeField] private float _lieTime = 1.6f;
        [SerializeField] private float _sinkTime = 1.4f;

        private readonly int _baseColorId = Shader.PropertyToID("_BaseColor");
        private MaterialPropertyBlock _block;
        private float _age;
        private float _scale;

        public void Show(Vector3 position, float yaw, Color tint, float scale)
        {
            transform.SetPositionAndRotation(new Vector3(position.x, 0f, position.z), Quaternion.Euler(0f, yaw, 0f));
            transform.localScale = Vector3.one * scale;
            _scale = scale;
            _age = 0f;

            _block ??= new MaterialPropertyBlock();
            _block.SetColor(_baseColorId, tint);
            _bodyRenderer.SetPropertyBlock(_block);
            Draw();
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_age >= _fallTime + _lieTime + _sinkTime)
            {
                Dismiss();
                return;
            }

            Draw();
        }

        private void Draw()
        {
            float fall = Mathf.Clamp01(_age / _fallTime);
            float sink = Mathf.Clamp01((_age - _fallTime - _lieTime) / _sinkTime);

            _body.localRotation = Quaternion.Euler(90f * fall * fall, 0f, 0f);
            _body.localPosition = new Vector3(0f, Mathf.Lerp(1f, 0.3f, fall) - sink * 1.2f, 0f);
            _blood.localScale = new Vector3(1.6f, 0.01f, 1.6f) * Mathf.Lerp(0.3f, 1f, fall) * (1f - sink);
        }
    }
}
