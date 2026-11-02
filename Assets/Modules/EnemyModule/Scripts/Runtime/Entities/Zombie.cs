using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>
    /// One zombie. The server moves it where the module's commands say and its NetworkTransform
    /// carries that to every client; it decides nothing itself. A hit flashes it and bursts blood on
    /// every screen.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class Zombie : NetworkBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        [SerializeField] private ParticleSystem _hitEffect;
        [Tooltip("Multiplies the model's colours, so a zombie never reads as a player.")]
        [SerializeField] private Color _skinTint = new(0.45f, 0.85f, 0.3f, 1f);
        [SerializeField] private Color _flashColor = new(1f, 0.15f, 0.15f, 1f);
        [SerializeField] private float _flashDuration = 0.12f;
        [Tooltip("How much bigger a hit swells the zombie for a moment.")]
        [SerializeField] private float _hitPunch = 0.18f;

        // The kind's look, set by the server before the zombie spawns and carried to every client.
        private readonly NetworkVariable<Color> _tint = new(Color.white);
        private readonly NetworkVariable<float> _scale = new(1f);

        private Renderer[] _renderers;
        private MaterialPropertyBlock _block;
        private Coroutine _flash;

        public Vector3 Position => transform.position;

        private void Awake()
        {
            // The tag is how a touchscreen player's auto-aim finds zombies without knowing this module.
            gameObject.tag = "Zombie";
            _renderers = GetComponentsInChildren<Renderer>(true);
            _block = new MaterialPropertyBlock();
            SetTint(_skinTint);
        }

        /// <summary>Server, before spawning: the colour and size of this zombie's kind.</summary>
        public void SetLookServer(Color tint, float scale)
        {
            _tint.Value = tint;
            _scale.Value = scale;
        }

        // Out of the pool, a zombie takes its kind's look whenever the server sets it.
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _tint.OnValueChanged += OnTintChanged;
            _scale.OnValueChanged += OnScaleChanged;
            transform.localScale = Vector3.one * _scale.Value;
            SetTint(_tint.Value);
        }

        public override void OnNetworkDespawn()
        {
            _tint.OnValueChanged -= OnTintChanged;
            _scale.OnValueChanged -= OnScaleChanged;
            if (_flash != null)
                StopCoroutine(_flash);
            _flash = null;
            transform.localScale = Vector3.one * _scale.Value;
            base.OnNetworkDespawn();
        }

        private void OnTintChanged(Color previous, Color current) => SetTint(current);

        private void OnScaleChanged(float previous, float current) => transform.localScale = Vector3.one * current;

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

        /// <summary>Server only: pushes the zombie back along the round's path and shows the hit to everyone.</summary>
        public void TakeHitServer(Vector3 from, float knockback)
        {
            if (!IsServer)
                return;

            Vector3 away = transform.position - from;
            away.y = 0f;
            if (away.sqrMagnitude > 0.0001f)
                transform.position += away.normalized * knockback;

            ShowHitClientRpc();
        }

        public void DespawnServer()
        {
            if (IsServer && NetworkObject.IsSpawned)
                NetworkObject.Despawn();
        }

        [ClientRpc]
        private void ShowHitClientRpc()
        {
            if (_hitEffect != null)
                _hitEffect.Play();

            if (_flash != null)
                StopCoroutine(_flash);

            _flash = StartCoroutine(Flash());
        }

        // A flash of colour and a swell that springs back.
        private IEnumerator Flash()
        {
            SetTint(_flashColor);
            float elapsed = 0f;
            float length = Mathf.Max(_flashDuration * 1.5f, 0.01f);
            while (elapsed < length)
            {
                float t = elapsed / length;
                transform.localScale = Vector3.one * (_scale.Value * (1f + _hitPunch * (1f - t) * (1f - t)));
                if (elapsed >= _flashDuration)
                    SetTint(_tint.Value);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = Vector3.one * _scale.Value;
            SetTint(_tint.Value);
            _flash = null;
        }

        private void SetTint(Color color)
        {
            foreach (Renderer target in _renderers)
            {
                if (target == null)
                    continue;

                target.GetPropertyBlock(_block);
                _block.SetColor(BaseColorId, color);
                _block.SetColor(ColorId, color);
                target.SetPropertyBlock(_block);
            }
        }
    }
}
