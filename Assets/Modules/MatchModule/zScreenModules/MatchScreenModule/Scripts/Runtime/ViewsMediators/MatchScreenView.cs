using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using Modules.MatchModule.Shared.Data.ValueObjects;
using Modules.PlayerModule.Shared.Data.ValueObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.MatchModule.MatchScreenModule.ViewsMediators
{
    /// <summary>
    /// The match overlay: the scoreboard and the button that leaves the match. Leaving asks first,
    /// on a small panel the view builds for itself.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class MatchScreenView : ScreenView
    {
        private const string CONFIRM_QUESTION = "Maçtan çıkmak istiyor musun?";
        private const string CONFIRM_YES = "Evet";
        private const string CONFIRM_NO = "Hayır";
        private const string GAME_OVER_TITLE = "OYUN BİTTİ";
        private const string GAME_OVER_BODY = "Bütün oyuncular düştü.";
        private const string GAME_OVER_LEAVE = "Ana Menü";
        private const string SETTINGS = "AYARLAR";
        private const string SETTINGS_CLOSE = "Kapat";
        private const string HEIGHT_LABEL = "Kamera yüksekliği";
        private const string FOLLOW_LABEL = "Kamera takip hızı";
        private const string SHAKE_LABEL = "Ateş sarsıntısı";

        [SerializeField] private Transform _rowContent;
        [SerializeField] private ScoreboardRowView _rowPrefab;
        [SerializeField] private Button _exitButton;

        private readonly List<ScoreboardRowView> _rows = new();

        private GameObject _confirmPanel;
        private Button _confirmYes;
        private Button _confirmNo;

        private GameObject _gameOverPanel;
        private Button _gameOverLeave;

        private Button _settingsButton;
        private GameObject _settingsPanel;
        private Button _settingsClose;
        private Slider _heightSlider;
        private Slider _followSlider;
        private Slider _shakeSlider;
        private TMP_Text _heightValue;
        private TMP_Text _followValue;
        private TMP_Text _shakeValue;

        /// <summary>A camera slider moved: height, follow speed, shake.</summary>
        public Action<float, float, float> CameraSettingsEdited;

        /// <summary>The exit button was pressed; the Mediator asks for confirmation.</summary>
        public Action ExitClicked;

        /// <summary>The player answered yes on the confirmation panel.</summary>
        public Action ExitConfirmed;

        private void Awake()
        {
            BuildConfirmPanel();
            BuildGameOverPanel();
            BuildSettings();
        }

        private void OnEnable()
        {
            _exitButton.onClick.AddListener(OnExitClicked);
            _confirmYes.onClick.AddListener(OnConfirmYes);
            _confirmNo.onClick.AddListener(HideExitConfirm);
            _gameOverLeave.onClick.AddListener(OnConfirmYes);
            _settingsButton.onClick.AddListener(ToggleSettings);
            _settingsClose.onClick.AddListener(HideSettings);
            _heightSlider.onValueChanged.AddListener(OnSliderMoved);
            _followSlider.onValueChanged.AddListener(OnSliderMoved);
            _shakeSlider.onValueChanged.AddListener(OnSliderMoved);
        }

        private void OnDisable()
        {
            _exitButton.onClick.RemoveListener(OnExitClicked);
            _confirmYes.onClick.RemoveListener(OnConfirmYes);
            _confirmNo.onClick.RemoveListener(HideExitConfirm);
            _gameOverLeave.onClick.RemoveListener(OnConfirmYes);
            _settingsButton.onClick.RemoveListener(ToggleSettings);
            _settingsClose.onClick.RemoveListener(HideSettings);
            _heightSlider.onValueChanged.RemoveListener(OnSliderMoved);
            _followSlider.onValueChanged.RemoveListener(OnSliderMoved);
            _shakeSlider.onValueChanged.RemoveListener(OnSliderMoved);
        }

        // A pooled screen comes back with whatever its last opening left; the question is never left up.
        public override void BeforeScreenActivation()
        {
            base.BeforeScreenActivation();
            HideExitConfirm();
            HideSettings();
            _gameOverPanel.SetActive(false);
        }

        /// <summary>The sliders take the camera's ranges and values without reporting a change back.</summary>
        public void ShowCameraSettings(CameraSettingsVO settings)
        {
            SetSlider(_heightSlider, settings.HeightMin, settings.HeightMax, settings.Height);
            SetSlider(_followSlider, settings.FollowSpeedMin, settings.FollowSpeedMax, settings.FollowSpeed);
            SetSlider(_shakeSlider, 0f, settings.ShakeMax, settings.Shake);
            DrawSliderValues();
        }

        private static void SetSlider(Slider slider, float min, float max, float value)
        {
            slider.minValue = min;
            slider.maxValue = max;
            slider.SetValueWithoutNotify(value);
        }

        private void ToggleSettings() => _settingsPanel.SetActive(!_settingsPanel.activeSelf);

        private void HideSettings()
        {
            if (_settingsPanel != null)
                _settingsPanel.SetActive(false);
        }

        private void OnSliderMoved(float value)
        {
            DrawSliderValues();
            CameraSettingsEdited?.Invoke(_heightSlider.value, _followSlider.value, _shakeSlider.value);
        }

        private void DrawSliderValues()
        {
            _heightValue.text = _heightSlider.value.ToString("0.0");
            _followValue.text = _followSlider.value.ToString("0.0");
            _shakeValue.text = $"{Mathf.RoundToInt(_shakeSlider.value * 100f)}%";
        }

        // The board stays readable under the panel; leaving is the only way on.
        public void ShowGameOver()
        {
            HideExitConfirm();
            _gameOverPanel.SetActive(true);
        }

        public void ShowScores(ScoreEntryVO[] scores)
        {
            // Rows are kept and reused; the board only ever holds the players of one match.
            while (_rows.Count < scores.Length)
                _rows.Add(Instantiate(_rowPrefab, _rowContent));

            for (int i = 0; i < _rows.Count; i++)
            {
                bool used = i < scores.Length;
                _rows[i].gameObject.SetActive(used);

                if (used)
                    _rows[i].Show(scores[i].PlayerName, scores[i].Kills, scores[i].Deaths, scores[i].Kda);
            }
        }

        public void ShowExitConfirm() => _confirmPanel.SetActive(true);

        public void HideExitConfirm()
        {
            if (_confirmPanel != null)
                _confirmPanel.SetActive(false);
        }

        private void OnExitClicked() => ExitClicked?.Invoke();

        private void OnConfirmYes()
        {
            HideExitConfirm();
            ExitConfirmed?.Invoke();
        }

        private void BuildConfirmPanel()
        {
            // A full-screen shade catches every click, so nothing behind the question is pressed.
            _confirmPanel = CreateRect("ExitConfirm", transform, Vector2.zero, Vector2.one, Vector2.zero);
            _confirmPanel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);

            GameObject box = CreateRect("Box", _confirmPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(360f, 170f));
            box.AddComponent<Image>().color = new Color(0.12f, 0.12f, 0.14f, 0.95f);

            GameObject question = CreateRect("Question", box.transform, new Vector2(0f, 0.5f), new Vector2(1f, 1f), Vector2.zero);
            TMP_Text questionText = question.AddComponent<TextMeshProUGUI>();
            questionText.text = CONFIRM_QUESTION;
            questionText.fontSize = 24f;
            questionText.alignment = TextAlignmentOptions.Center;

            _confirmYes = CreateButton("Yes", box.transform, new Vector2(0.27f, 0.25f), CONFIRM_YES, new Color(0.7f, 0.2f, 0.2f));
            _confirmNo = CreateButton("No", box.transform, new Vector2(0.73f, 0.25f), CONFIRM_NO, new Color(0.3f, 0.3f, 0.34f));

            _confirmPanel.SetActive(false);
        }

        // The button sits under the exit button; the panel opens beside it and leaves the arena in view.
        private void BuildSettings()
        {
            GameObject buttonGo = CreateRect("SettingsButton", transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(170f, 48f));
            var buttonRect = (RectTransform) buttonGo.transform;
            buttonRect.pivot = new Vector2(0f, 1f);
            buttonRect.anchoredPosition = new Vector2(75f, -125f);
            buttonGo.AddComponent<Image>().color = new Color(0.12f, 0.12f, 0.14f, 0.85f);
            _settingsButton = buttonGo.AddComponent<Button>();
            AddLabel(buttonGo.transform, SETTINGS, 22f);

            _settingsPanel = CreateRect("SettingsPanel", transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(460f, 300f));
            var panelRect = (RectTransform) _settingsPanel.transform;
            panelRect.pivot = new Vector2(0f, 1f);
            panelRect.anchoredPosition = new Vector2(75f, -185f);
            _settingsPanel.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.1f, 0.92f);

            _heightSlider = CreateSettingRow(_settingsPanel.transform, HEIGHT_LABEL, -30f, out _heightValue);
            _followSlider = CreateSettingRow(_settingsPanel.transform, FOLLOW_LABEL, -105f, out _followValue);
            _shakeSlider = CreateSettingRow(_settingsPanel.transform, SHAKE_LABEL, -180f, out _shakeValue);

            _settingsClose = CreateButton("Close", _settingsPanel.transform, new Vector2(0.5f, 0f), SETTINGS_CLOSE, new Color(0.3f, 0.3f, 0.34f));
            ((RectTransform) _settingsClose.transform).anchoredPosition = new Vector2(0f, 36f);

            _settingsPanel.SetActive(false);
        }

        private static Slider CreateSettingRow(Transform parent, string label, float y, out TMP_Text valueText)
        {
            GameObject row = CreateRect(label, parent, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-40f, 64f));
            var rowRect = (RectTransform) row.transform;
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.anchoredPosition = new Vector2(0f, y);

            GameObject title = CreateRect("Label", row.transform, new Vector2(0f, 0.55f), new Vector2(0.75f, 1f), Vector2.zero);
            TMP_Text titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = label;
            titleText.fontSize = 20f;
            titleText.alignment = TextAlignmentOptions.MidlineLeft;

            GameObject value = CreateRect("Value", row.transform, new Vector2(0.75f, 0.55f), new Vector2(1f, 1f), Vector2.zero);
            valueText = value.AddComponent<TextMeshProUGUI>();
            valueText.fontSize = 20f;
            valueText.alignment = TextAlignmentOptions.MidlineRight;
            valueText.color = new Color(1f, 0.75f, 0.35f);

            return CreateSlider(row.transform);
        }

        // uGUI's slider, put together by hand: a track, a fill and a round handle.
        private static Slider CreateSlider(Transform parent)
        {
            GameObject go = CreateRect("Slider", parent, new Vector2(0f, 0f), new Vector2(1f, 0.45f), Vector2.zero);

            GameObject track = CreateRect("Track", go.transform, new Vector2(0f, 0.35f), new Vector2(1f, 0.65f), Vector2.zero);
            track.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.15f);

            GameObject fillArea = CreateRect("FillArea", go.transform, new Vector2(0f, 0.35f), new Vector2(1f, 0.65f), Vector2.zero);
            GameObject fill = CreateRect("Fill", fillArea.transform, Vector2.zero, Vector2.one, Vector2.zero);
            fill.AddComponent<Image>().color = new Color(0.95f, 0.5f, 0.2f);

            GameObject handleArea = CreateRect("HandleArea", go.transform, Vector2.zero, Vector2.one, new Vector2(-24f, 0f));
            GameObject handle = CreateRect("Handle", handleArea.transform, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(24f, 0f));
            var handleImage = handle.AddComponent<Image>();
            handleImage.sprite = DiscSprite();

            var slider = go.AddComponent<Slider>();
            slider.fillRect = (RectTransform) fill.transform;
            slider.handleRect = (RectTransform) handle.transform;
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            return slider;
        }

        // Unity's built-in knob sprite is not there in every build, so the handle draws its own circle.
        private static Sprite DiscSprite()
        {
            const int size = 64;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[size * size];
            float half = size * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(half, half));
                    byte alpha = (byte) (Mathf.Clamp01(half - distance) * 255f);
                    pixels[y * size + x] = new Color32(255, 255, 255, alpha);
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f));
        }

                private static void AddLabel(Transform parent, string label, float size)
        {
            GameObject text = CreateRect("Label", parent, Vector2.zero, Vector2.one, Vector2.zero);
            TMP_Text labelText = text.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = size;
            labelText.fontStyle = FontStyles.Bold;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.raycastTarget = false;
        }

        private void BuildGameOverPanel()
        {
            _gameOverPanel = CreateRect("GameOver", transform, Vector2.zero, Vector2.one, Vector2.zero);
            _gameOverPanel.AddComponent<Image>().color = new Color(0.25f, 0f, 0f, 0.55f);

            GameObject box = CreateRect("Box", _gameOverPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(420f, 220f));
            box.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.1f, 0.95f);

            GameObject title = CreateRect("Title", box.transform, new Vector2(0f, 0.62f), new Vector2(1f, 1f), Vector2.zero);
            TMP_Text titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = GAME_OVER_TITLE;
            titleText.fontSize = 44f;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(0.95f, 0.3f, 0.25f);
            titleText.alignment = TextAlignmentOptions.Center;

            GameObject body = CreateRect("Body", box.transform, new Vector2(0f, 0.4f), new Vector2(1f, 0.62f), Vector2.zero);
            TMP_Text bodyText = body.AddComponent<TextMeshProUGUI>();
            bodyText.text = GAME_OVER_BODY;
            bodyText.fontSize = 22f;
            bodyText.alignment = TextAlignmentOptions.Center;

            _gameOverLeave = CreateButton("Leave", box.transform, new Vector2(0.5f, 0.2f), GAME_OVER_LEAVE, new Color(0.7f, 0.2f, 0.2f));
            ((RectTransform) _gameOverLeave.transform).sizeDelta = new Vector2(180f, 52f);

            _gameOverPanel.SetActive(false);
        }

        private static Button CreateButton(string name, Transform parent, Vector2 anchor, string label, Color color)
        {
            GameObject go = CreateRect(name, parent, anchor, anchor, new Vector2(130f, 48f));
            go.AddComponent<Image>().color = color;
            var button = go.AddComponent<Button>();

            GameObject text = CreateRect("Label", go.transform, Vector2.zero, Vector2.one, Vector2.zero);
            TMP_Text labelText = text.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 22f;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.raycastTarget = false;

            return button;
        }

        private static GameObject CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform) go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            return go;
        }

        protected override void PlayShowAnimation() => ShowCompleted?.Invoke(this);

        protected override void PlayHideAnimation() => HideCompleted?.Invoke(this);
    }
}
