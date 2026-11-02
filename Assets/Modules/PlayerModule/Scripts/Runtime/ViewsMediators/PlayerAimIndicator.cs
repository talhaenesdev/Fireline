using Unity.Netcode;
using UnityEngine;

namespace Modules.PlayerModule.ViewsMediators
{
    /// <summary>
    /// A flat arrow on the ground in front of the avatar, so everyone can read which way it is aiming.
    /// It turns with the avatar, which already faces the aim, and decides nothing, so it has no Mediator.
    /// </summary>
    public class PlayerAimIndicator : MonoBehaviour
    {
        [SerializeField] private float _start = 0.6f;
        [SerializeField] private float _length = 3.2f;
        [SerializeField] private float _width = 0.12f;
        [SerializeField] private float _headLength = 0.45f;
        [SerializeField] private float _headWidth = 0.42f;
        [SerializeField] private float _height = 0.05f;
        [SerializeField] private Color _localColour = new(0.3f, 0.95f, 1f, 0.75f);
        [SerializeField] private Color _remoteColour = new(1f, 0.85f, 0.35f, 0.6f);

        private NetworkObject _networkObject;
        private PlayerAvatarView _avatar;
        private MeshRenderer _renderer;
        private Material _material;
        private bool _isLocal;

        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
            _avatar = GetComponent<PlayerAvatarView>();
            Build();
        }

        private void Update()
        {
            bool isLocal = _networkObject != null && _networkObject.IsSpawned && _networkObject.IsOwner;
            if (isLocal != _isLocal)
            {
                _isLocal = isLocal;
                _material.color = _isLocal ? _localColour : _remoteColour;
            }

            // A body on the ground aims nowhere.
            bool show = _avatar == null || !_avatar.IsDead;
            if (_renderer.gameObject.activeSelf != show)
                _renderer.gameObject.SetActive(show);
        }

        private void OnDestroy()
        {
            if (_material != null)
                Destroy(_material);
        }

        private void Build()
        {
            var go = new GameObject("AimIndicator");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0f, _height, 0f);

            float shaftEnd = _start + _length - _headLength;
            float halfWidth = _width * 0.5f;
            float halfHead = _headWidth * 0.5f;

            // The shaft fades in from the feet; the head is solid.
            var mesh = new Mesh { name = "AimIndicator" };
            mesh.vertices = new[]
            {
                new Vector3(-halfWidth, 0f, _start), new Vector3(halfWidth, 0f, _start),
                new Vector3(-halfWidth, 0f, shaftEnd), new Vector3(halfWidth, 0f, shaftEnd),
                new Vector3(-halfHead, 0f, shaftEnd), new Vector3(halfHead, 0f, shaftEnd),
                new Vector3(0f, 0f, _start + _length)
            };
            mesh.colors = new[]
            {
                new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 0f),
                Color.white, Color.white, Color.white, Color.white, Color.white
            };
            mesh.triangles = new[] { 0, 2, 1, 1, 2, 3, 4, 6, 5 };
            mesh.RecalculateBounds();

            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            _renderer = go.AddComponent<MeshRenderer>();
            _renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _renderer.receiveShadows = false;

            _material = new Material(Shader.Find("Sprites/Default")) { color = _remoteColour };
            _renderer.sharedMaterial = _material;
        }
    }
}
