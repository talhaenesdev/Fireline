using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using Modules.EconomyModule.Shared.Data.ValueObjects;
using Modules.EconomyModule.Shared.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.EconomyModule.ShopScreenModule.ViewsMediators
{
    /// <summary>
    /// The shop: a panel on the right listing what money buys, open while the player stands in the
    /// shop zone. The rows are built from the items it is given; a row the player cannot afford
    /// is greyed out.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class ShopScreenView : ScreenView
    {
        private const string TITLE = "MAĞAZA";
        private const string BUY = "Al";

        private RectTransform _panel;
        private TMP_Text _moneyText;
        private readonly List<Row> _rows = new();
        private ShopItemCVO[] _items = Array.Empty<ShopItemCVO>();
        private int _money;

        /// <summary>Buy was pressed on this item, by its place in the list.</summary>
        public Action<int> BuyClicked;

        private class Row
        {
            public GameObject Root;
            public TMP_Text Label;
            public Button Button;
        }

        private void Awake() => BuildPanel();

        private void OnDisable()
        {
            foreach (Row row in _rows)
                row.Button.onClick.RemoveAllListeners();
        }

        private void OnEnable() => WireRows();

        public void ShowItems(ShopItemCVO[] items)
        {
            _items = items ?? Array.Empty<ShopItemCVO>();

            while (_rows.Count < _items.Length)
                _rows.Add(CreateRow(_rows.Count));

            for (int i = 0; i < _rows.Count; i++)
                _rows[i].Root.SetActive(i < _items.Length);

            _panel.sizeDelta = new Vector2(_panel.sizeDelta.x, 110f + _items.Length * 64f);
            WireRows();
            Draw();
        }

        public void ShowMoney(int money)
        {
            _money = money;
            Draw();
        }

        private void WireRows()
        {
            for (int i = 0; i < _rows.Count; i++)
            {
                int item = i;
                _rows[i].Button.onClick.RemoveAllListeners();
                _rows[i].Button.onClick.AddListener(() => BuyClicked?.Invoke(item));
            }
        }

        private void Draw()
        {
            if (_moneyText == null)
                return;

            _moneyText.text = $"Para: {_money}";
            for (int i = 0; i < _items.Length && i < _rows.Count; i++)
            {
                ShopItemCVO item = _items[i];
                bool affordable = _money >= item.Price;
                _rows[i].Label.text = $"{item.Name}  <color=#9AA4B2><size=80%>{Describe(item)}</size></color>\n<color=#FFC857>{item.Price}</color>";
                _rows[i].Button.interactable = affordable;
            }
        }

        private static string Describe(ShopItemCVO item) => item.Type switch
        {
            ShopItemType.Heal => $"+{item.Amount:0} can",
            ShopItemType.Shield => $"+{item.Amount:0} kalkan",
            ShopItemType.FireRate => $"%{item.Amount * 100f:0} daha hızlı ateş",
            ShopItemType.Turret => "önüne kurulur, zombilere ateş eder",
            _ => string.Empty
        };

        private void BuildPanel()
        {
            _panel = Rect("Panel", transform, new Vector2(1f, 0.5f), new Vector2(-30f, 0f), new Vector2(380f, 300f));
            _panel.pivot = new Vector2(1f, 0.5f);
            _panel.gameObject.AddComponent<Image>().color = new Color(0.07f, 0.08f, 0.1f, 0.93f);

            TMP_Text title = Text("Title", _panel, TITLE, 32f, FontStyles.Bold);
            Place(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -30f), new Vector2(-40f, 44f));
            title.color = new Color(0.4f, 0.75f, 1f);

            _moneyText = Text("Money", _panel, string.Empty, 22f, FontStyles.Bold);
            Place(_moneyText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -70f), new Vector2(-40f, 30f));
            _moneyText.color = new Color(1f, 0.78f, 0.34f);
        }

        private Row CreateRow(int index)
        {
            RectTransform row = Rect($"Item_{index}", _panel, new Vector2(0.5f, 1f), new Vector2(0f, -110f - index * 64f), new Vector2(340f, 56f));
            row.pivot = new Vector2(0.5f, 1f);
            row.gameObject.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.06f);

            TMP_Text label = Text("Label", row, string.Empty, 20f, FontStyles.Normal);
            Place(label.rectTransform, Vector2.zero, new Vector2(0.72f, 1f), new Vector2(12f, 0f), new Vector2(-12f, 0f));
            label.alignment = TextAlignmentOptions.MidlineLeft;

            RectTransform buttonRect = Rect("Buy", row, new Vector2(1f, 0.5f), new Vector2(-8f, 0f), new Vector2(80f, 40f));
            buttonRect.pivot = new Vector2(1f, 0.5f);
            var image = buttonRect.gameObject.AddComponent<Image>();
            image.color = new Color(0.25f, 0.6f, 0.3f);
            var button = buttonRect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.disabledColor = new Color(0.35f, 0.35f, 0.35f, 0.6f);
            button.colors = colors;

            TMP_Text buy = Text("Label", buttonRect, BUY, 20f, FontStyles.Bold);
            Place(buy.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            buy.raycastTarget = false;

            return new Row { Root = row.gameObject, Label = label, Button = button };
        }

        private static TMP_Text Text(string name, Transform parent, string text, float size, FontStyles style)
        {
            RectTransform rect = Rect(name, parent, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = size;
            label.fontStyle = style;
            label.alignment = TextAlignmentOptions.Center;
            return label;
        }

        private static void Place(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
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

        protected override void PlayShowAnimation() => ShowCompleted?.Invoke(this);

        protected override void PlayHideAnimation() => HideCompleted?.Invoke(this);
    }
}
