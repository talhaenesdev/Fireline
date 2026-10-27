using System;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Modules.UiWidgetsModule.ViewsMediators
{
    /// <summary>
    /// A button's hover and click, for its sound. A button that cannot be pressed makes none. It
    /// sits on screen prefabs the screen service instantiates, so its ViewInjector names UiWidgetsSystemRoot.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class UiButtonAudioView : MonoBehaviour, IView, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private Button _targetButton;

        public bool IsRegistered { get; set; }

        public Action Hovered;
        public Action Clicked;

        private void Awake()
        {
            if (_targetButton == null)
                _targetButton = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsPressable())
                Hovered?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsPressable())
                Clicked?.Invoke();
        }

        private bool IsPressable() => _targetButton != null && _targetButton.enabled && _targetButton.interactable;
    }
}
