using System;
using TMPro;
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
        private Graphic _knobGraphic;
        private float _radius;

        public Action<Vector2> Moved;

        public void Configure(RectTransform knob)
        {
            _area = (RectTransform) transform;
            _knob = knob;
            _knobGraphic = knob.GetComponent<Graphic>();
            _radius = _area.sizeDelta.x * 0.5f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Press(true);
            OnDrag(eventData);
        }

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
            Press(false);

            if (_knob != null)
                _knob.anchoredPosition = Vector2.zero;

            Moved?.Invoke(Vector2.zero);
        }

        private void Press(bool isPressed)
        {
            if (_knob != null)
                _knob.localScale = Vector3.one * (isPressed ? 1.1f : 1f);

            if (_knobGraphic != null)
                _knobGraphic.color = new Color(1f, 1f, 1f, isPressed ? 0.95f : 0.7f);
        }
    }

    /// <summary>An on-screen button that reports held and released, the way a trigger does.</summary>
    public class TouchHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Action<bool> HeldChanged;

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.localScale = Vector3.one * 0.92f;
            HeldChanged?.Invoke(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.localScale = Vector3.one;
            HeldChanged?.Invoke(false);
        }

        private void OnDisable()
        {
            transform.localScale = Vector3.one;
            HeldChanged?.Invoke(false);
        }
    }

    /// <summary>
    /// Builds the HUD's touch controls and its wave label; how they look is all it decides. The
    /// circles are drawn here, smooth at any size, rather than taken from Unity's small built-in knob.
    /// </summary>
    public static class TouchControlsBuilder
    {
        private const int CIRCLE_SIZE = 256;

        public static TouchJoystick Joystick(Transform parent)
        {
            RectTransform area = Rect("MoveStick", parent, new Vector2(0f, 0f), new Vector2(190f, 190f), new Vector2(220f, 220f));
            Circle(area.gameObject, Disc(), new Color(0f, 0f, 0f, 0.35f));
            Child("Ring", area, Vector2.zero, Ring(), new Color(1f, 1f, 1f, 0.55f));

            RectTransform knob = Child("Knob", area, new Vector2(96f, 96f), Disc(), new Color(1f, 1f, 1f, 0.7f));
            Child("KnobRing", knob, Vector2.zero, Ring(), new Color(1f, 1f, 1f, 0.9f));

            var stick = area.gameObject.AddComponent<TouchJoystick>();
            stick.Configure(knob);
            return stick;
        }

        public static TouchHoldButton HoldButton(Transform parent, string name, Vector2 position, float size, Color color, string label)
        {
            RectTransform rect = RoundButton(parent, name, position, size, color, label);
            return rect.gameObject.AddComponent<TouchHoldButton>();
        }

        public static Button TapButton(Transform parent, string name, Vector2 position, float size, Color color, string label)
        {
            RectTransform rect = RoundButton(parent, name, position, size, color, label);
            var button = rect.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            return button;
        }

        /// <summary>A line of text at the top centre of the HUD, for the wave.</summary>
        public static TMP_Text TopLabel(Transform parent, string name)
        {
            RectTransform rect = Rect(name, parent, new Vector2(0.5f, 1f), new Vector2(0f, -70f), new Vector2(600f, 100f));
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = 40f;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(1f, 0.85f, 0.4f);
            text.outlineWidth = 0.2f;
            text.outlineColor = new Color32(0, 0, 0, 200);
            text.raycastTarget = false;
            return text;
        }

        /// <summary>Money and bag, top left under the match buttons, clear of the phone's stick.</summary>
        public static TMP_Text CornerLabel(Transform parent, string name)
        {
            RectTransform rect = Rect(name, parent, new Vector2(0f, 1f), new Vector2(75f, -190f), new Vector2(300f, 70f));
            rect.pivot = new Vector2(0f, 1f);
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = 26f;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.TopLeft;
            text.outlineWidth = 0.2f;
            text.outlineColor = new Color32(0, 0, 0, 200);
            text.raycastTarget = false;
            return text;
        }

        /// <summary>A wide button at the bottom centre, above the phone's controls.</summary>
        public static Button WideButton(Transform parent, string name, string label)
        {
            RectTransform rect = Rect(name, parent, new Vector2(0.5f, 0f), new Vector2(0f, 150f), new Vector2(320f, 70f));
            var image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.2f, 0.6f, 0.3f, 0.9f);
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            RectTransform labelRect = Rect("Label", rect, Vector2.zero, Vector2.zero, Vector2.zero);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            var text = labelRect.gameObject.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 26f;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            return button;
        }

        /// <summary>A round button with a ring that fills as the revive goes on.</summary>
        public static Button ReviveButton(Transform parent, out Image progress, out TMP_Text label)
        {
            RectTransform rect = Rect("ReviveButton", parent, Vector2.zero, Vector2.zero, new Vector2(130f, 130f));
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            Image disc = Circle(rect.gameObject, Disc(), new Color(0.15f, 0.65f, 0.35f, 0.85f));
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = disc;

            RectTransform ring = Child("Progress", rect, Vector2.zero, Ring(), new Color(1f, 1f, 1f, 0.95f));
            progress = ring.GetComponent<Image>();
            progress.type = Image.Type.Filled;
            progress.fillMethod = Image.FillMethod.Radial360;
            progress.fillOrigin = (int) Image.Origin360.Top;
            progress.fillClockwise = true;
            progress.fillAmount = 0f;

            RectTransform labelRect = Rect("Label", rect, Vector2.zero, Vector2.zero, Vector2.zero);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            label = labelRect.gameObject.AddComponent<TextMeshProUGUI>();
            label.fontSize = 20f;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            return button;
        }

        private static RectTransform RoundButton(Transform parent, string name, Vector2 position, float size, Color color, string label)
        {
            RectTransform rect = Rect(name, parent, new Vector2(1f, 0f), position, new Vector2(size, size));
            Circle(rect.gameObject, Disc(), new Color(color.r, color.g, color.b, 0.55f));
            Child("Ring", rect, Vector2.zero, Ring(), new Color(1f, 1f, 1f, 0.75f));

            RectTransform labelRect = Rect("Label", rect, Vector2.zero, Vector2.zero, Vector2.zero);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            var text = labelRect.gameObject.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = size * 0.22f;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            return rect;
        }

        // A child filling its parent when size is zero, otherwise centred at that size.
        private static RectTransform Child(string name, RectTransform parent, Vector2 size, Sprite sprite, Color color)
        {
            RectTransform rect = Rect(name, parent, new Vector2(0.5f, 0.5f), Vector2.zero, size);
            if (size == Vector2.zero)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
            }

            Circle(rect.gameObject, sprite, color).raycastTarget = false;
            return rect;
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

        private static Image Circle(GameObject go, Sprite sprite, Color color)
        {
            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            return image;
        }

        private static Sprite Disc() => CircleSprite(0f);

        private static Sprite Ring() => CircleSprite(0.88f);

        // White, anti-aliased: a full disc, or a ring whose inner edge sits at innerRadius of the outer.
        private static Sprite CircleSprite(float innerRadius)
        {
            var texture = new Texture2D(CIRCLE_SIZE, CIRCLE_SIZE, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            float half = CIRCLE_SIZE * 0.5f;
            float edge = 1.5f / half;
            var pixels = new Color32[CIRCLE_SIZE * CIRCLE_SIZE];

            for (int y = 0; y < CIRCLE_SIZE; y++)
            {
                for (int x = 0; x < CIRCLE_SIZE; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(half, half)) / half;
                    float alpha = Mathf.Clamp01((1f - distance) / edge);
                    if (innerRadius > 0f)
                        alpha *= Mathf.Clamp01((distance - innerRadius) / edge);

                    pixels[y * CIRCLE_SIZE + x] = new Color32(255, 255, 255, (byte) (alpha * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0f, 0f, CIRCLE_SIZE, CIRCLE_SIZE), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
