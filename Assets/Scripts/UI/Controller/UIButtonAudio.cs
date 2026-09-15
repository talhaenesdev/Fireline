using FireLine.Scripts.UI.Model;
using FireLine.Scripts.UI.Service;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace FireLine.Scripts.UI.Controller
{
    public class UIButtonAudio :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerClickHandler
    {
        [Header("Button")]
        [SerializeField]
        private UIButtonType buttonType =
            UIButtonType.Default;

        [Header("References")]
        [SerializeField]
        private Button targetButton;

        private IUIAudioService _audioService;

        [Inject]
        public void Initialize(
            IUIAudioService audioService)
        {
            _audioService = audioService;
        }

        private void Awake()
        {
            if (targetButton == null)
            {
                targetButton =
                    GetComponent<Button>();
            }

            if (targetButton == null)
            {
                Debug.LogError(
                    $"[UI BUTTON AUDIO] " +
                    $"Button not found | " +
                    $"Object={gameObject.name}"
                );
            }
        }

        public void OnPointerEnter(
            PointerEventData eventData)
        {
            if (!CanPlay())
                return;

            _audioService.PlayHover(
                buttonType
            );
        }

        public void OnPointerClick(
            PointerEventData eventData)
        {
            if (!CanPlay())
                return;

            _audioService.PlayClick(
                buttonType
            );
        }

        private bool CanPlay()
        {
            if (targetButton == null)
                return false;

            if (!targetButton.enabled)
                return false;

            if (!targetButton.interactable)
                return false;

            if (_audioService == null)
                return false;

            return true;
        }
    }
}