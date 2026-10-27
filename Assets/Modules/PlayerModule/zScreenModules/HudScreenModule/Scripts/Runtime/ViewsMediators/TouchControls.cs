using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Modules.PlayerModule.HudScreenModule.ViewsMediators
{
    /// <summary>An on-screen stick: drag inside its circle to move, let go to stop.</summary>
    public class TouchJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private RectTransform _area;
        private RectTransform _knob;
        private float _radius;

        public Action<Vector2> Moved;

        public void Configure(RectTransform knob)
        {
            _area = (RectTransform) transform;
            _knob = knob;
            _radius = _area.sizeDelta.x * 0.5f;
        }

        public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

        public void OnDrag(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_area, eventData.position, eventData.pressEventCamera, out Vector2 local))
                return;

            Vector2 offset = Vector2.ClampMagnitude(local, _radius);
            _knob.anchoredPosition = offset;
            Moved?.Invoke(offset / _radius);
        }

        public void OnPointerUp(PointerEventData eventData) => Release();

        private void OnDisable() => Release();

        private void Release()
        {
            if (_knob != null)
                _knob.anchoredPosition = Vector2.zero;

            Moved?.Invoke(Vector2.zero);
        }
    }

    /// <summary>An on-screen button that reports held and released, the way a trigger does.</summary>
    public class TouchHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Action<bool> HeldChanged;

        public void OnPointerDown(PointerEventData eventData) => HeldChanged?.Invoke(true);

        public void OnPointerUp(PointerEventData eventData) => HeldChanged?.Invoke(false);

        private void OnDisable() => HeldChanged?.Invoke(false);
    }

    /// <summary>Builds the HUD's touch controls; how they look is all it decides.</summary>
    public static class TouchControlsBuilder
    {
        public static TouchJoystick Joystick(Transform parent)
        {
            RectTransform area = Rect("MoveStick", parent, new Vector2(0f, 0f), new Vector2(170f, 170f), new Vector2(150f, 150f));
            Circle(area.gameObject, new Color(1f, 1f, 1f, 0.15f));

            RectTransform knob = Rect("Knob", area, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(80f, 80f));
            Circle(knob.gameObject, new Color(1f, 1f, 1f, 0.45f)).raycastTarget = false;

            var stick = area.gameObject.AddComponent<TouchJoystick>();
            stick.Configure(knob);
            return stick;
        }

        public static TouchHoldButton HoldButton(Transform parent, string name, Vector2 position, float size, Color color)
        {
            RectTransform rect = Rect(name, parent, new Vector2(1f, 0f), position, new Vector2(size, size));
            Circle(rect.gameObject, color);
            return rect.gameObject.AddComponent<TouchHoldButton>();
        }

        public static Button TapButton(Transform parent, string name, Vector2 position, float size, Color color)
        {
            RectTransform rect = Rect(name, parent, new Vector2(1f, 0f), position, new Vector2(size, size));
            Circle(rect.gameObject, color);
            return rect.gameObject.AddComponent<Button>();
        }

        private static RectTransform Rect(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform) go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        private static Image Circle(GameObject go, Color color)
        {
            var image = go.AddComponent<Image>();
            image.color = color;
            image.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
            return image;
        }
    }
}
