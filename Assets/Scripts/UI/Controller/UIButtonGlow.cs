using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FireLine.Scripts.UI.Controller
{
    public class UIButtonGlow :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        [Header("References")]
        [SerializeField]
        private Button targetButton;

        [SerializeField]
        private RawImage glowImage;

        [Header("Hover Glow")]
        [SerializeField]
        private float minAlpha = 0.05f;

        [SerializeField]
        private float maxAlpha = 0.65f;

        [SerializeField]
        private float pulseSpeed = 3f;

        [Header("Click Glow")]
        [SerializeField]
        private float clickAlpha = 2.5f;

        [SerializeField]
        private float clickFadeSpeed = 8f;

        private bool _isHovered;
        private bool _isPressed;

        private float _time;
        private float _currentAlpha;

        private void Awake()
        {
            if (targetButton == null)
                targetButton = GetComponent<Button>();

            if (glowImage == null)
                glowImage = GetComponentInChildren<RawImage>(true);

            SetGlowAlpha(0f);
        }

        private void Update()
        {
            if (!CanGlow())
            {
                _isHovered = false;
                _isPressed = false;

                SetGlowAlpha(0f);
                return;
            }

            // Tıklama sırasında aşırı parlak glow
            if (_isPressed)
            {
                _currentAlpha = Mathf.MoveTowards(
                    _currentAlpha,
                    clickAlpha,
                    clickFadeSpeed * Time.deltaTime
                );

                SetGlowAlpha(_currentAlpha);
                return;
            }

            // Mouse üzerindeyse normal pulse
            if (_isHovered)
            {
                _time += Time.deltaTime * pulseSpeed;

                float pulse =
                    (Mathf.Sin(_time) + 1f) * 0.5f;

                float targetAlpha =
                    Mathf.Lerp(
                        minAlpha,
                        maxAlpha,
                        pulse
                    );

                _currentAlpha = Mathf.MoveTowards(
                    _currentAlpha,
                    targetAlpha,
                    clickFadeSpeed * Time.deltaTime
                );

                SetGlowAlpha(_currentAlpha);
                return;
            }

            _currentAlpha = Mathf.MoveTowards(
                _currentAlpha,
                0f,
                clickFadeSpeed * Time.deltaTime
            );

            SetGlowAlpha(_currentAlpha);
        }

        public void OnPointerEnter(
            PointerEventData eventData)
        {
            if (!CanGlow())
                return;

            _isHovered = true;
            _time = 0f;
        }

        public void OnPointerExit(
            PointerEventData eventData)
        {
            _isHovered = false;
            _isPressed = false;
        }

        public void OnPointerDown(
            PointerEventData eventData)
        {
            if (!CanGlow())
                return;

            _isPressed = true;

            // Tıklama anında direkt güçlü parlaklık
            _currentAlpha = clickAlpha;

            SetGlowAlpha(_currentAlpha);
        }

        public void OnPointerUp(
            PointerEventData eventData)
        {
            _isPressed = false;
        }

        private bool CanGlow()
        {
            if (!gameObject.activeInHierarchy)
                return false;

            if (targetButton == null)
                return false;

            if (!targetButton.enabled)
                return false;

            if (!targetButton.interactable)
                return false;

            return true;
        }

        private void SetGlowAlpha(float alpha)
        {
            if (glowImage == null)
                return;

            Color color = glowImage.color;
            color.a = alpha;
            glowImage.color = color;
        }
    }
}