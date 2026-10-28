using System;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.PlayerModule.HudScreenModule.ViewsMediators
{
    [RequireComponent(typeof(ViewInjector))]
    public class HudScreenView : ScreenView
    {
        [SerializeField] private RawImage _healthFill;
        [Tooltip("Where the bar starts and ends across the fill texture, 0 to 1; the clear margin around it is left out.")]
        [SerializeField] private Vector2 _healthFillRange = new(0.033f, 0.967f);
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TMP_Text _ammoText;

        public Action<Vector2> TouchMoved;
        public Action<bool, bool> TouchFireChanged;
        public Action TouchReloadPressed;

        /// <summary>The revive button over a downed teammate was pressed.</summary>
        public Action ReviveClicked;

        /// <summary>The place-turret button was pressed.</summary>
        public Action PlaceTurretClicked;

        private Button _placeTurretButton;

        private TouchJoystick _stick;
        private TouchHoldButton _fire;
        private Button _reload;

        private RectTransform _healthFillRect;
        private float _healthFillWidth;
        private float _healthFillLeft;

        private TMP_Text _waveText;
        private int _wave;
        private int _zombiesLeft;
        private float _waveStartTime;
        private int _shownSeconds = -1;

        private float _health;
        private float _maxHealth;
        private float _shield;
        private TMP_Text _walletText;

        private RectTransform _reviveRect;
        private Button _reviveButton;
        private Image _reviveProgress;
        private TMP_Text _reviveLabel;

        // A phone has no keyboard or mouse; it plays from controls drawn on the HUD.
        private void Awake()
        {
            PrepareHealthFill();
            _waveText = TouchControlsBuilder.TopLabel(transform, "WaveText");
            _waveText.gameObject.SetActive(false);

            _walletText = TouchControlsBuilder.CornerLabel(transform, "WalletText");
            _walletText.gameObject.SetActive(false);

            _reviveButton = TouchControlsBuilder.ReviveButton(transform, out _reviveProgress, out _reviveLabel);
            _reviveRect = (RectTransform) _reviveButton.transform;
            _reviveButton.gameObject.SetActive(false);

            _placeTurretButton = TouchControlsBuilder.WideButton(transform, "PlaceTurretButton", "TARETİ YERLEŞTİR");
            _placeTurretButton.gameObject.SetActive(false);

            if (!Application.isMobilePlatform)
                return;

            _stick = TouchControlsBuilder.Joystick(transform);
            _fire = TouchControlsBuilder.HoldButton(transform, "FireButton", new Vector2(-160f, 170f), 170f, new Color(0.9f, 0.27f, 0.2f), "ATEŞ");
            _reload = TouchControlsBuilder.TapButton(transform, "ReloadButton", new Vector2(-320f, 110f), 100f, new Color(0.95f, 0.75f, 0.25f), "R");
        }

        // The fill is a picture, not a filled Image: it is cut from the right, picture and rect together.
        private void PrepareHealthFill()
        {
            if (_healthFill == null)
                return;

            _healthFillRect = _healthFill.rectTransform;
            _healthFillWidth = _healthFillRect.rect.width;
            _healthFillLeft = _healthFillRect.anchoredPosition.x - _healthFillWidth * _healthFillRect.pivot.x;
            _healthFillRect.pivot = new Vector2(0f, _healthFillRect.pivot.y);
            _healthFillRect.anchoredPosition = new Vector2(_healthFillLeft, _healthFillRect.anchoredPosition.y);
        }

        private void Update()
        {
            if (_wave <= 0 || _waveStartTime <= 0f)
                return;

            int seconds = Mathf.CeilToInt(_waveStartTime - Time.time);
            if (seconds == _shownSeconds)
                return;

            _shownSeconds = seconds;
            DrawWave();
        }

        private void OnEnable()
        {
            _reviveButton.onClick.AddListener(OnReviveClicked);
            _placeTurretButton.onClick.AddListener(OnPlaceTurretClicked);

            if (_stick == null)
                return;

            _stick.Moved += OnStickMoved;
            _fire.HeldChanged += OnFireHeld;
            _reload.onClick.AddListener(OnReloadClicked);
        }

        private void OnDisable()
        {
            _reviveButton.onClick.RemoveListener(OnReviveClicked);
            _placeTurretButton.onClick.RemoveListener(OnPlaceTurretClicked);

            if (_stick == null)
                return;

            _stick.Moved -= OnStickMoved;
            _fire.HeldChanged -= OnFireHeld;
            _reload.onClick.RemoveListener(OnReloadClicked);
        }

        private void OnStickMoved(Vector2 move) => TouchMoved?.Invoke(move);
        private void OnFireHeld(bool isHeld) => TouchFireChanged?.Invoke(isHeld, isHeld);
        private void OnReloadClicked() => TouchReloadPressed?.Invoke();
        private void OnReviveClicked() => ReviveClicked?.Invoke();
        private void OnPlaceTurretClicked() => PlaceTurretClicked?.Invoke();

        public void ShowTurretPlacement(bool isPlacing) => _placeTurretButton.gameObject.SetActive(isPlacing);

        public void ShowHealth(float health, float maxHealth)
        {
            if (maxHealth <= 0f)
                return;

            _health = health;
            _maxHealth = maxHealth;

            if (_healthFillRect != null)
            {
                float ratio = Mathf.Clamp01(health / maxHealth);
                float cut = _healthFillRange.x + (_healthFillRange.y - _healthFillRange.x) * ratio;
                _healthFill.uvRect = new Rect(0f, 0f, cut, 1f);
                _healthFillRect.sizeDelta = new Vector2(_healthFillWidth * cut, _healthFillRect.sizeDelta.y);
            }

            DrawHealthText();
        }

        public void ShowShield(float shield)
        {
            _shield = shield;
            DrawHealthText();
        }

        private void DrawHealthText()
        {
            if (_healthText == null || _maxHealth <= 0f)
                return;

            string health = $"{Mathf.CeilToInt(_health)} / {Mathf.CeilToInt(_maxHealth)}";
            _healthText.text = _shield > 0f ? $"{health}  <color=#6EC6FF>+{Mathf.CeilToInt(_shield)}</color>" : health;
        }

        public void ShowWallet(int money, int carried)
        {
            _walletText.gameObject.SetActive(true);
            _walletText.text = $"<color=#FFC857>Para: {money}</color>\n<size=80%>Çanta: {carried}</size>";
        }

        /// <summary>
        /// Puts the revive button over the downed teammate, where the camera sees them, and fills
        /// its ring as the revive goes on. It follows the teammate on screen every frame.
        /// </summary>
        public void ShowRevive(bool hasTarget, Vector3 worldPosition, float progress)
        {
            Camera view = Camera.main;
            if (!hasTarget || view == null)
            {
                if (_reviveButton.gameObject.activeSelf)
                    _reviveButton.gameObject.SetActive(false);
                return;
            }

            Vector3 screen = view.WorldToScreenPoint(worldPosition + Vector3.up * 2f);
            if (screen.z < 0f)
            {
                _reviveButton.gameObject.SetActive(false);
                return;
            }

            if (!_reviveButton.gameObject.activeSelf)
                _reviveButton.gameObject.SetActive(true);

            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) transform, screen, null, out Vector2 local);
            _reviveRect.anchoredPosition = local;

            bool isReviving = progress >= 0f;
            _reviveProgress.fillAmount = isReviving ? progress : 0f;
            _reviveButton.interactable = !isReviving;
            _reviveLabel.text = isReviving ? "..." : "CANLANDIR";
        }

        public void ShowAmmo(int ammo, int magazineSize)
        {
            if (_ammoText != null)
                _ammoText.text = $"{ammo} / {magazineSize}";
        }

        /// <summary>Counts down to the wave while it is coming, then shows the zombies left in it.</summary>
        public void ShowWave(int wave, float secondsUntilWave, int zombiesLeft)
        {
            _wave = wave;
            _zombiesLeft = zombiesLeft;
            _waveStartTime = secondsUntilWave > 0f ? Time.time + secondsUntilWave : 0f;
            _shownSeconds = -1;
            DrawWave();
        }

        private void DrawWave()
        {
            _waveText.gameObject.SetActive(_wave > 0);
            if (_wave <= 0)
                return;

            int seconds = _waveStartTime > 0f ? Mathf.CeilToInt(_waveStartTime - Time.time) : 0;
            _waveText.text = seconds > 0
                ? $"DALGA {_wave}\n<size=70%>{seconds} sn sonra geliyor</size>"
                : $"DALGA {_wave}\n<size=70%>Kalan zombi: {_zombiesLeft}</size>";
        }

        protected override void PlayShowAnimation() => ShowCompleted?.Invoke(this);

        protected override void PlayHideAnimation() => HideCompleted?.Invoke(this);
    }
}
