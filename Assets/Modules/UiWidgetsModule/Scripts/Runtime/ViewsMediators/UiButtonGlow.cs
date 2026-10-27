using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Modules.UiWidgetsModule.ViewsMediators
{
    /// <summary>
    /// A glow that pulses while a button is hovered and flares while it is pressed. It is how the
    /// button looks and decides nothing, so it has no Mediator.
    /// </summary>
    public class UiButtonGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Button _targetButton;
        [SerializeField] private RawImage _glowImage;

        [Header("Hover")]
        [SerializeField] private float _minAlpha = 0.05f;
        [SerializeField] private float _maxAlpha = 0.65f;
        [SerializeField] private float _pulseSpeed = 3f;

        [Header("Press")]
        [SerializeField] private float _clickAlpha = 2.5f;
        [SerializeField] private float _clickFadeSpeed = 8f;

        private bool _isHovered;
        private bool _isPressed;
        private float _time;
        private float _currentAlpha;

        private void Awake()
        {
            if (_targetButton == null)
                _targetButton = GetComponent<Button>();

            if (_glowImage == null)
                _glowImage = GetComponentInChildren<RawImage>(true);

            SetAlpha(0f);
        }

        private void OnDisable()
        {
            _isHovered = false;
            _isPressed = false;
            _currentAlpha = 0f;
            SetAlpha(0f);
        }

        private void Update()
        {
            if (!CanGlow())
            {
                _isHovered = false;
                _isPressed = false;
                SetAlpha(0f);
                return;
            }

            float target = 0f;
            if (_isPressed)
            {
                target = _clickAlpha;
            }
            else if (_isHovered)
            {
                _time += Time.deltaTime * _pulseSpeed;
                target = Mathf.Lerp(_minAlpha, _maxAlpha, (Mathf.Sin(_time) + 1f) * 0.5f);
            }

            _currentAlpha = Mathf.MoveTowards(_currentAlpha, target, _clickFadeSpeed * Time.deltaTime);
            SetAlpha(_currentAlpha);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!CanGlow())
                return;

            _isHovered = true;
            _time = 0f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            _isPressed = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanGlow())
                return;

            _isPressed = true;
            _currentAlpha = _clickAlpha;
            SetAlpha(_currentAlpha);
        }

        public void OnPointerUp(PointerEventData eventData) => _isPressed = false;

        private bool CanGlow() =>
            gameObject.activeInHierarchy && _targetButton != null && _targetButton.enabled && _targetButton.interactable;

        private void SetAlpha(float alpha)
        {
            if (_glowImage == null)
                return;

            Color color = _glowImage.color;
            color.a = alpha;
            _glowImage.color = color;
        }
    }
}
