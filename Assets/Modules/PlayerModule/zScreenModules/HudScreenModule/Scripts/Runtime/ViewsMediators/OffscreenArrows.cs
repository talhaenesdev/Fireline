using System.Collections.Generic;
using Modules.PlayerModule.Shared.Data.ValueObjects;
using Modules.PlayerModule.Shared.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.PlayerModule.HudScreenModule.ViewsMediators
{
    /// <summary>
    /// Arrows at the edge of the HUD pointing at what the camera does not see: red for zombies, green
    /// for a downed teammate, blue for a ping. The arrows are kept and reused, so a busy wave makes none.
    /// </summary>
    public class OffscreenArrows
    {
        private const float MARGIN = 60f;
        private const int MAX_ZOMBIE_ARROWS = 12;

        private readonly RectTransform _root;
        private readonly List<Image> _arrows = new();
        private Sprite _sprite;

        public OffscreenArrows(RectTransform parent)
        {
            var go = new GameObject("OffscreenArrows", typeof(RectTransform));
            _root = (RectTransform) go.transform;
            _root.SetParent(parent, false);
            _root.anchorMin = Vector2.zero;
            _root.anchorMax = Vector2.one;
            _root.offsetMin = _root.offsetMax = Vector2.zero;
        }

        public void Show(MarkerVO[] markers, Camera view)
        {
            int used = 0;
            int zombies = 0;

            if (view != null)
            {
                Rect area = _root.rect;
                Vector2 half = area.size * 0.5f - Vector2.one * MARGIN;

                foreach (MarkerVO marker in markers)
                {
                    if (marker.Kind == MarkerKind.Zombie && ++zombies > MAX_ZOMBIE_ARROWS)
                        continue;

                    Vector3 viewport = view.WorldToViewportPoint(marker.Position);
                    if (viewport.z > 0f && viewport.x is > 0f and < 1f && viewport.y is > 0f and < 1f)
                        continue;

                    Vector2 direction = new Vector2(viewport.x - 0.5f, viewport.y - 0.5f);
                    if (viewport.z < 0f)
                        direction = -direction;
                    if (direction.sqrMagnitude < 0.0001f)
                        continue;

                    // Out along the direction until the edge of the HUD, less a margin.
                    float scale = Mathf.Min(half.x / Mathf.Max(0.0001f, Mathf.Abs(direction.x)), half.y / Mathf.Max(0.0001f, Mathf.Abs(direction.y)));
                    Image arrow = Arrow(used++);
                    arrow.rectTransform.anchoredPosition = direction * scale;
                    arrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
                    arrow.rectTransform.sizeDelta = marker.Kind == MarkerKind.Zombie ? new Vector2(26f, 26f) : new Vector2(40f, 40f);
                    arrow.color = Colour(marker.Kind);
                }
            }

            for (int i = used; i < _arrows.Count; i++)
            {
                if (_arrows[i].gameObject.activeSelf)
                    _arrows[i].gameObject.SetActive(false);
            }
        }

        private Image Arrow(int index)
        {
            while (_arrows.Count <= index)
            {
                var go = new GameObject("Arrow", typeof(RectTransform));
                var rect = (RectTransform) go.transform;
                rect.SetParent(_root, false);
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                var image = go.AddComponent<Image>();
                image.sprite = _sprite ??= ArrowSprite();
                image.raycastTarget = false;
                _arrows.Add(image);
            }

            Image arrow = _arrows[index];
            if (!arrow.gameObject.activeSelf)
                arrow.gameObject.SetActive(true);
            return arrow;
        }

        private static Color Colour(MarkerKind kind) => kind switch
        {
            MarkerKind.DownedTeammate => new Color(0.35f, 1f, 0.45f, 0.95f),
            MarkerKind.Ping => new Color(0.35f, 0.75f, 1f, 0.95f),
            _ => new Color(1f, 0.25f, 0.2f, 0.8f)
        };

        // A white triangle pointing up.
        private static Sprite ArrowSprite()
        {
            const int size = 64;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            {
                float halfWidth = (1f - (y + 0.5f) / size) * size * 0.5f;
                for (int x = 0; x < size; x++)
                {
                    float edge = Mathf.Clamp01(halfWidth - Mathf.Abs(x + 0.5f - size * 0.5f));
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte) (edge * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
