using FlowIoC.PoolModule.Entities;
using TMPro;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>A hit's damage, floating up from the zombie and fading; a heavy hit is larger and gold.</summary>
    public class DamageNumber : PoolableItem
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _lifetime = 0.8f;
        [SerializeField] private float _rise = 1.4f;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private Color _heavyColor = new(1f, 0.8f, 0.2f, 1f);
        [SerializeField] private float _heavyScale = 1.5f;

        private Vector3 _start;
        private float _age;
        private Color _shown;
        private float _scale;

        public void Show(Vector3 position, int damage, bool isHeavy)
        {
            Vector2 scatter = Random.insideUnitCircle * 0.3f;
            _start = position + new Vector3(scatter.x, 2f, scatter.y);
            _age = 0f;
            _shown = isHeavy ? _heavyColor : _color;
            _scale = isHeavy ? _heavyScale : 1f;
            _text.text = damage.ToString();
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
            transform.SetPositionAndRotation(_start + Vector3.up * (_rise * t), Quaternion.Euler(90f, 0f, 0f));
            // A pop at the start, then it settles.
            transform.localScale = Vector3.one * (_scale * (1f + 0.4f * Mathf.Clamp01(1f - t * 5f)));
            _text.color = new Color(_shown.r, _shown.g, _shown.b, 1f - t * t);
        }
    }
}
