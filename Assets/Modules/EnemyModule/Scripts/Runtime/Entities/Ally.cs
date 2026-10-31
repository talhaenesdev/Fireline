using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>
    /// A soldier a barracks trains. The server moves it where the module's commands say and its
    /// NetworkTransform carries that to every client; it decides nothing itself. It has no collider:
    /// zombies find it by its position, and the players' rounds pass through it.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class Ally : NetworkBehaviour
    {
        private const float SHOT_TIME = 0.06f;
        private const float TRACER_WIDTH = 0.04f;
        private const float MUZZLE_HEIGHT = 1.2f;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        [SerializeField] private Transform _tracer;
        [SerializeField] private Color _flashColor = new(1f, 0.2f, 0.2f, 1f);
        [SerializeField] private float _flashDuration = 0.12f;

        private readonly NetworkVariable<Color> _tint = new(Color.white);

        private Renderer[] _renderers;
        private MaterialPropertyBlock _block;
        private Coroutine _flash;
        private float _tracerUntil;

        public Vector3 Position => transform.position;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            _block = new MaterialPropertyBlock();
        }

        /// <summary>Server, before spawning: the colour the barracks dresses its soldiers in.</summary>
        public void SetLookServer(Color tint) => _tint.Value = tint;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _tint.OnValueChanged += OnTintChanged;
            if (_tracer != null)
                _tracer.gameObject.SetActive(false);
            SetTint(_tint.Value);
        }

        public override void OnNetworkDespawn()
        {
            _tint.OnValueChanged -= OnTintChanged;
            if (_flash != null)
                StopCoroutine(_flash);
            _flash = null;
            base.OnNetworkDespawn();
        }

        private void OnTintChanged(Color previous, Color current) => SetTint(current);

        public void StepTowards(Vector3 target, float distance)
        {
            Vector3 direction = target - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
                return;

            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += direction.normalized * Mathf.Min(distance, direction.magnitude);
        }

        public void Face(Vector3 target)
        {
            Vector3 direction = target - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(direction);
        }

        /// <summary>Server: every client sees the soldier fire at this point.</summary>
        public void ShowShotServer(Vector3 target)
        {
            if (IsServer)
                ShotClientRpc(target);
        }

        /// <summary>Server: every client sees the soldier take a hit.</summary>
        public void ShowHitServer()
        {
            if (IsServer)
                HitClientRpc();
        }

        public void DespawnServer()
        {
            if (IsServer && NetworkObject.IsSpawned)
                NetworkObject.Despawn();
        }

        [ClientRpc]
        private void ShotClientRpc(Vector3 target)
        {
            if (_tracer == null)
                return;

            Vector3 from = transform.position + Vector3.up * MUZZLE_HEIGHT;
            Vector3 line = target + Vector3.up - from;
            if (line.sqrMagnitude < 0.01f)
                return;

            _tracer.SetPositionAndRotation(from + line * 0.5f, Quaternion.LookRotation(line));
            _tracer.localScale = new Vector3(TRACER_WIDTH, TRACER_WIDTH, line.magnitude);
            _tracer.gameObject.SetActive(true);
            _tracerUntil = Time.time + SHOT_TIME;
        }

        [ClientRpc]
        private void HitClientRpc()
        {
            if (_flash != null)
                StopCoroutine(_flash);

            _flash = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            SetTint(_flashColor);
            yield return new WaitForSeconds(_flashDuration);
            SetTint(_tint.Value);
            _flash = null;
        }

        private void SetTint(Color color)
        {
            if (_renderers == null)
                return;

            foreach (Renderer target in _renderers)
            {
                if (target == null || (_tracer != null && target.transform.IsChildOf(_tracer)))
                    continue;

                target.GetPropertyBlock(_block);
                _block.SetColor(BaseColorId, color);
                _block.SetColor(ColorId, color);
                target.SetPropertyBlock(_block);
            }
        }

        private void Update()
        {
            if (_tracer != null && _tracer.gameObject.activeSelf && Time.time > _tracerUntil)
                _tracer.gameObject.SetActive(false);
        }
    }
}
