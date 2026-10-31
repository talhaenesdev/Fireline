using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>
    /// A turret as each client draws it, taken from the Turret pool: a base, a head that turns to what
    /// it fires at, a health bar and a tracer flashed for each shot.
    /// </summary>
    public class TurretVisual : PoolableItem, IStructureVisual
    {
        private const float SHOT_TIME = 0.06f;
        private const float MUZZLE_OFFSET = 0.6f;
        private const float TRACER_WIDTH = 0.05f;

        [SerializeField] private Transform _head;
        [SerializeField] private Transform _healthBar;
        [SerializeField] private Transform _healthFill;
        [SerializeField] private Transform _tracer;
        [Tooltip("Tinted by the turret's level.")]
        [SerializeField] private Renderer _headBody;

        private readonly int _baseColorId = Shader.PropertyToID("_BaseColor");
        private MaterialPropertyBlock _block;

        private float _tracerUntil;

        public override void OnGetFromPool()
        {
            transform.localScale = Vector3.one;
            _head.localRotation = Quaternion.identity;
            _healthBar.gameObject.SetActive(true);
            _tracer.gameObject.SetActive(false);
            SetHealth(1f);
        }

        public void Place(Vector3 position, float scale, bool showsHealth)
        {
            transform.SetPositionAndRotation(position, Quaternion.identity);
            transform.localScale = Vector3.one * scale;
            _healthBar.gameObject.SetActive(showsHealth);
        }

        public void SetState(float yaw, float healthShare, Color color)
        {
            _head.rotation = Quaternion.Euler(0f, yaw, 0f);
            SetHealth(healthShare);

            if (_headBody == null)
                return;

            _block ??= new MaterialPropertyBlock();
            _block.SetColor(_baseColorId, color);
            _headBody.SetPropertyBlock(_block);
        }

        public void ShowShot(Vector3 target)
        {
            Vector3 from = _head.position + _head.forward * MUZZLE_OFFSET;
            Vector3 line = target + Vector3.up - from;
            if (line.sqrMagnitude < 0.01f)
                return;

            _tracer.SetPositionAndRotation(from + line * 0.5f, Quaternion.LookRotation(line));
            _tracer.localScale = new Vector3(TRACER_WIDTH, TRACER_WIDTH, line.magnitude);
            _tracer.gameObject.SetActive(true);
            _tracerUntil = Time.time + SHOT_TIME;
        }

        private void SetHealth(float share)
        {
            _healthFill.localScale = new Vector3(share, 1f, 1f);
            _healthFill.localPosition = new Vector3((share - 1f) * 0.5f, 0f, 0f);
        }

        private void Update()
        {
            if (_tracer.gameObject.activeSelf && Time.time > _tracerUntil)
                _tracer.gameObject.SetActive(false);
        }
    }
}
