using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using Unity.Netcode;
using UnityEngine;

namespace Modules.PlayerModule.ViewsMediators
{
    /// <summary>Moves the avatar by the step it is given and keeps the animator in step with it.</summary>
    [RequireComponent(typeof(ViewInjector))]
    public class PlayerMotorView : MonoBehaviour, IView
    {
        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
        private static readonly int MovementStateHash = Animator.StringToHash("Movement");

        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _aimTransform;
        [SerializeField] private int _upperBodyLayerIndex = 1;
        [Tooltip("A remote avatar moving faster than this, in metres a second, shows its sprint.")]
        [SerializeField] private float _remoteSprintSpeed = 6.5f;

        private NetworkObject _networkObject;
        private Vector3 _lastPosition;
        private Vector3 _remoteVelocity;

        public bool IsRegistered { get; set; }

        private void Awake()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();

            if (_aimTransform == null)
                _aimTransform = transform;

            _networkObject = GetComponent<NetworkObject>();
            _lastPosition = transform.position;
        }

        // A remote avatar is moved by its NetworkTransform and gets no input here, so its legs
        // follow how fast and which way it is actually moving.
        private void Update()
        {
            Vector3 position = transform.position;
            Vector3 delta = position - _lastPosition;
            _lastPosition = position;

            if (_networkObject == null || !_networkObject.IsSpawned || _networkObject.IsOwner || Time.deltaTime <= 0f)
                return;

            delta.y = 0f;
            _remoteVelocity = Vector3.Lerp(_remoteVelocity, delta / Time.deltaTime, 1f - Mathf.Exp(-12f * Time.deltaTime));

            float speed = _remoteVelocity.magnitude;
            Vector2 input = speed < 0.2f ? Vector2.zero : new Vector2(_remoteVelocity.x, _remoteVelocity.z) / speed;
            ShowMovement(input, speed > _remoteSprintSpeed);
        }

        public void Move(Vector3 step, Vector2 input, bool isSprinting)
        {
            transform.position += step;
            ShowMovement(input, isSprinting);
        }

        public void ShowMovement(Vector2 input, bool isSprinting)
        {
            if (_animator == null)
                return;

            if (input.sqrMagnitude < 0.001f)
            {
                _animator.SetFloat(MoveXHash, 0f);
                _animator.SetFloat(MoveYHash, 0f);
                _animator.SetFloat(SpeedHash, 0f);
                return;
            }

            // The animation blends in the avatar's own frame, so a strafe reads as a strafe.
            Vector3 localMovement = _aimTransform.InverseTransformDirection(new Vector3(input.x, 0f, input.y));
            _animator.SetFloat(MoveXHash, localMovement.x);
            _animator.SetFloat(MoveYHash, localMovement.z);
            _animator.SetFloat(SpeedHash, isSprinting ? 2f : 1f);
        }

        public void ShowDeath(bool isDead)
        {
            if (_animator == null)
                return;

            _animator.SetBool(IsDeadHash, isDead);

            // The death state has no way out, so a revived avatar is put back on its feet.
            if (!isDead && _animator.HasState(0, MovementStateHash))
                _animator.Play(MovementStateHash, 0, 0f);

            if (_upperBodyLayerIndex >= 0 && _upperBodyLayerIndex < _animator.layerCount)
                _animator.SetLayerWeight(_upperBodyLayerIndex, isDead ? 0f : 1f);
        }
    }
}
