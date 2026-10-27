using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
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

        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _aimTransform;
        [SerializeField] private int _upperBodyLayerIndex = 1;

        public bool IsRegistered { get; set; }

        private void Awake()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();

            if (_aimTransform == null)
                _aimTransform = transform;
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

            if (_upperBodyLayerIndex >= 0 && _upperBodyLayerIndex < _animator.layerCount)
                _animator.SetLayerWeight(_upperBodyLayerIndex, isDead ? 0f : 1f);
        }
    }
}
