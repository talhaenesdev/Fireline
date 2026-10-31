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

        /// <summary>The ping button was pressed.</summary>
        public Action PingClicked;

        private Button _ping;
        private OffscreenArrows _arrows;

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
        private int _money;
        private int _carried;
        private string _progress = string.Empty;
        private string _objective = string.Empty;

        private const float BANNER_TIME = 2.6f;
        private const float HURT_FADE = 0.45f;

        private TMP_Text _banner;
        private float _bannerUntil;
        private Image _hurt;
        private RectTransform _bossBar;
        private RectTransform _bossFill;
        private TMP_Text _bossLabel;
        private float _hurtAlpha;

        private RectTransform _reviveRect;
        private Button _reviveButton;
        private Image _reviveProgress;
        private TMP_Text _reviveLabel;

        // A phone has no keyboard or mouse; it plays from controls drawn on the HUD.
        private void Awake()
        {
            PrepareHealthFill();
            _hurt = TouchControlsBuilder.Vignette(transform);
            _banner = TouchControlsBuilder.Banner(transform, "Banner");
            _banner.gameObject.SetActive(false);
            _bossBar = TouchControlsBuilder.BossBar(transform, out _bossFill, out _bossLabel);
            _bossBar.gameObject.SetActive(false);
            _waveText = TouchControlsBuilder.TopLabel(transform, "WaveText");
            _waveText.gameObject.SetActive(false);

            _walletText = TouchControlsBuilder.CornerLabel(transform, "WalletText");
            _walletText.gameObject.SetActive(false);

            _reviveButton = TouchControlsBuilder.ReviveButton(transform, out _reviveProgress, out _reviveLabel);
            _reviveRect = (RectTransform) _reviveButton.transform;
            _reviveButton.gameObject.SetActive(false);

            _placeTurretButton = TouchControlsBuilder.WideButton(transform, "PlaceTurretButton", "YERLEŞTİR");
            _placeTurretButton.gameObject.SetActive(false);
            _arrows = new OffscreenArrows((RectTransform) transform);

            if (!Application.isMobilePlatform)
                return;

            _stick = TouchControlsBuilder.Joystick(transform);
            _fire = TouchControlsBuilder.HoldButton(transform, "FireButton", new Vector2(-160f, 170f), 170f, new Color(0.9f, 0.27f, 0.2f), "ATEŞ");
            _reload = TouchControlsBuilder.TapButton(transform, "ReloadButton", new Vector2(-320f, 110f), 100f, new Color(0.95f, 0.75f, 0.25f), "R");
            _ping = TouchControlsBuilder.TapButton(transform, "PingButton", new Vector2(-110f, 330f), 90f, new Color(0.3f, 0.75f, 1f), "PİNG");
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
            DrawFeedback();

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
            _ping.onClick.AddListener(OnPingClicked);
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
            _ping.onClick.RemoveListener(OnPingClicked);
        }

        private void OnStickMoved(Vector2 move) => TouchMoved?.Invoke(move);
        private void OnFireHeld(bool isHeld) => TouchFireChanged?.Invoke(isHeld, isHeld);
        private void OnReloadClicked() => TouchReloadPressed?.Invoke();
        private void OnReviveClicked() => ReviveClicked?.Invoke();
        private void OnPlaceTurretClicked() => PlaceTurretClicked?.Invoke();
        private void OnPingClicked() => PingClicked?.Invoke();

        public void ShowMarkers(Modules.PlayerModule.Shared.Data.ValueObjects.MarkerVO[] markers) => _arrows.Show(markers, Camera.main);

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
            _money = money;
            _carried = carried;
            DrawWallet();
        }

        public void ShowProgress(int level, int intoLevel, int levelSize)
        {
            _progress = $"\n<size=70%><color=#9AD0FF>Seviye {level}  ({intoLevel}/{levelSize} XP)</color></size>";
            DrawWallet();
        }

        private void DrawWallet()
        {
            _walletText.gameObject.SetActive(true);
            _walletText.text = $"<color=#FFC857>Para: {_money}</color>\n<size=80%>Çanta: {_carried}</size>{_progress}";
        }

        public void ShowObjective(string title, bool isActive, float progress)
        {
            _objective = isActive ? $"\n<size=55%><color=#7FE08A>GÖREV: {title}  %{Mathf.RoundToInt(progress * 100f)}</color></size>" : string.Empty;
            DrawWave();
        }

        public void ShowObjectiveEnded(bool isHeld, int reward) => ShowBanner(isHeld
            ? $"<color=#7FE08A>GÖREV TAMAM</color>\n<size=60%><color=#FFC857>+{reward} para</color></size>"
            : "<color=#FF5A5A>GÖREV BAŞARISIZ</color>");

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

        private string _weaponName;
        private int _ammo;
        private int _magazineSize;

        public void ShowAmmo(int ammo, int magazineSize)
        {
            _ammo = ammo;
            _magazineSize = magazineSize;
            DrawAmmo();
        }

        public void ShowWeaponName(string weaponName)
        {
            _weaponName = weaponName;
            DrawAmmo();
        }

        private void DrawAmmo()
        {
            if (_ammoText == null)
                return;

            string ammo = $"{_ammo} / {_magazineSize}";
            _ammoText.text = string.IsNullOrEmpty(_weaponName) ? ammo : $"<size=60%>{_weaponName}</size>\n{ammo}";
        }

        public void ShowHurt(float amount)
        {
            _hurtAlpha = Mathf.Clamp(_hurtAlpha + 0.25f + amount / 60f, 0f, 0.85f);
        }

        public void ShowWaveCleared(int wave, int reward) =>
            ShowBanner($"DALGA {wave} TEMİZLENDİ\n<size=60%><color=#FFC857>+{reward} para</color></size>");

        /// <summary>The boss's health across the top; a boss that just came in is announced.</summary>
        public void ShowBoss(string name, int health, int maxHealth)
        {
            bool isShown = !string.IsNullOrEmpty(name) && maxHealth > 0;
            if (isShown && !_bossBar.gameObject.activeSelf)
                ShowBanner($"<color=#FF5A5A>BOSS GELİYOR</color>\n<size=60%>{name}</size>");

            _bossBar.gameObject.SetActive(isShown);
            if (!isShown)
                return;

            _bossFill.anchorMax = new Vector2(Mathf.Clamp01((float) health / maxHealth), 1f);
            _bossLabel.text = $"{name}  {health} / {maxHealth}";
        }

        public void ShowBanner(string text)
        {
            _banner.text = text;
            _banner.gameObject.SetActive(true);
            _bannerUntil = Time.time + BANNER_TIME;
        }

        private void DrawFeedback()
        {
            if (_hurtAlpha > 0f)
            {
                _hurtAlpha = Mathf.Max(0f, _hurtAlpha - Time.deltaTime / HURT_FADE);
                _hurt.color = new Color(0.8f, 0f, 0f, _hurtAlpha);
            }

            if (!_banner.gameObject.activeSelf)
                return;

            float left = _bannerUntil - Time.time;
            if (left <= 0f)
            {
                _banner.gameObject.SetActive(false);
                return;
            }

            _banner.alpha = Mathf.Clamp01(left / 0.5f);
            _banner.transform.localScale = Vector3.one * (1f + 0.25f * Mathf.Clamp01((left - BANNER_TIME + 0.2f) / 0.2f));
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
                : $"DALGA {_wave}\n<size=70%>Kalan zombi: {_zombiesLeft}</size>{_objective}";
        }

        protected override void PlayShowAnimation() => ShowCompleted?.Invoke(this);

        protected override void PlayHideAnimation() => HideCompleted?.Invoke(this);
    }
}
