using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.EconomyModule.Entities
{
    /// <summary>A piece of loot on the ground, taken from the Loot pool: a box in the loot's colour, spinning and bobbing.</summary>
    public class LootVisual : PoolableItem
    {
        private const float SPIN = 90f;
        private const float BOB = 0.15f;
        private const float HEIGHT = 0.5f;

        [SerializeField] private Renderer _box;

        private readonly int _baseColorId = Shader.PropertyToID("_BaseColor");
        private MaterialPropertyBlock _block;

        public void Place(Vector3 position, Color color)
        {
            transform.SetPositionAndRotation(position, Quaternion.identity);

            _block ??= new MaterialPropertyBlock();
            _block.SetColor(_baseColorId, color);
            _box.SetPropertyBlock(_block);
        }

        private void Update()
        {
            Transform box = _box.transform;
            box.Rotate(0f, SPIN * Time.deltaTime, 0f, Space.World);
            box.localPosition = new Vector3(0f, HEIGHT + Mathf.Sin(Time.time * 3f) * BOB, 0f);
        }
    }
}
