using System;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using Modules.LocalizationModule.Entities;
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
        public Action RepairClicked;
        public Action OrderClicked;
        public Action SkipWaveClicked;

        /// <summary>Filled by the Mediator; every text on the HUD is looked up through it.</summary>
        public TextLocalizer Localizer { get; } = new();

        private Button _ping;
        private Button _repair;
        private Button _order;
        private OffscreenArrows _arrows;

        private Button _placeTurretButton;
        private Button _skipWaveButton;

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
        private int _materials;
        private bool _hasWallet;
        private int _moneyChange;
        private int _materialsChange;
        private float _walletChangedAt = -10f;
        private const float WALLET_POP = 0.25f;
        private const float WALLET_CHANGE_SHOWN = 1.4f;
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
            _arrows = new OffscreenArrows((RectTransform) transform, Localizer);
            BindLabel(_placeTurretButton, "YERLEŞTİR");

            // Under the countdown, only while the break is on.
            _skipWaveButton = TouchControlsBuilder.WideButton(transform, "SkipWaveButton", "DALGAYI BAŞLAT");
            var skipRect = (RectTransform) _skipWaveButton.transform;
            skipRect.anchorMin = skipRect.anchorMax = new Vector2(0.5f, 1f);
            skipRect.anchoredPosition = new Vector2(0f, -160f);
            skipRect.sizeDelta = new Vector2(280f, 54f);
            _skipWaveButton.gameObject.SetActive(false);
            BindLabel(_skipWaveButton, "DALGAYI BAŞLAT");
            Localizer.Changed += Redraw;

            if (!Application.isMobilePlatform)
                return;

            _stick = TouchControlsBuilder.Joystick(transform);
            _fire = TouchControlsBuilder.HoldButton(transform, "FireButton", new Vector2(-160f, 170f), 170f, new Color(0.9f, 0.27f, 0.2f), "ATEŞ");
            _reload = TouchControlsBuilder.TapButton(transform, "ReloadButton", new Vector2(-320f, 110f), 100f, new Color(0.95f, 0.75f, 0.25f), "R");
            _ping = TouchControlsBuilder.TapButton(transform, "PingButton", new Vector2(-110f, 330f), 90f, new Color(0.3f, 0.75f, 1f), "PİNG");
            _repair = TouchControlsBuilder.TapButton(transform, "RepairButton", new Vector2(-230f, 330f), 90f, new Color(0.55f, 0.85f, 0.4f), "TAMİR");
            _order = TouchControlsBuilder.TapButton(transform, "OrderButton", new Vector2(-350f, 330f), 90f, new Color(0.45f, 0.6f, 1f), "EMİR");
            BindLabel(_fire, "ATEŞ");
            BindLabel(_ping, "PİNG");
            BindLabel(_repair, "TAMİR");
            BindLabel(_order, "EMİR");
        }

        private void BindLabel(Component control, string text)
        {
            if (control != null)
                Localizer.Bind(control.GetComponentInChildren<TMP_Text>(true), text);
        }

        private string L(string text) => Localizer.Get(text);

        // The language changed: whatever the HUD composed is composed again.
        private void Redraw()
        {
            if (_hasWallet)
                DrawWallet();
            DrawWave();
            DrawAmmo();
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
            DrawWalletPop();

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
            _skipWaveButton.onClick.AddListener(OnSkipWaveClicked);

            if (_stick == null)
                return;

            _stick.Moved += OnStickMoved;
            _fire.HeldChanged += OnFireHeld;
            _reload.onClick.AddListener(OnReloadClicked);
            _ping.onClick.AddListener(OnPingClicked);
            _repair.onClick.AddListener(OnRepairClicked);
            _order.onClick.AddListener(OnOrderClicked);
        }

        private void OnDisable()
        {
            _reviveButton.onClick.RemoveListener(OnReviveClicked);
            _placeTurretButton.onClick.RemoveListener(OnPlaceTurretClicked);
            _skipWaveButton.onClick.RemoveListener(OnSkipWaveClicked);

            if (_stick == null)
                return;

            _stick.Moved -= OnStickMoved;
            _fire.HeldChanged -= OnFireHeld;
            _reload.onClick.RemoveListener(OnReloadClicked);
            _ping.onClick.RemoveListener(OnPingClicked);
            _repair.onClick.RemoveListener(OnRepairClicked);
            _order.onClick.RemoveListener(OnOrderClicked);
        }

        private void OnStickMoved(Vector2 move) => TouchMoved?.Invoke(move);
        private void OnFireHeld(bool isHeld) => TouchFireChanged?.Invoke(isHeld, isHeld);
        private void OnReloadClicked() => TouchReloadPressed?.Invoke();
        private void OnReviveClicked() => ReviveClicked?.Invoke();
        private void OnPlaceTurretClicked() => PlaceTurretClicked?.Invoke();
        private void OnPingClicked() => PingClicked?.Invoke();
        private void OnRepairClicked() => RepairClicked?.Invoke();
        private void OnOrderClicked() => OrderClicked?.Invoke();
        private void OnSkipWaveClicked() => SkipWaveClicked?.Invoke();

        public void ShowAllyOrder(bool isFollowing) =>
            ShowBanner(L(isFollowing ? "ASKERLER SENİ TAKİP EDİYOR" : "ASKERLER NÖBETTE"));

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

        public void ShowWallet(int money, int carried, int materials)
        {
            // What came in or went out shows beside the totals for a moment, and the line swells.
            if (_hasWallet && (money != _money || materials != _materials))
            {
                bool isFresh = Time.time - _walletChangedAt < WALLET_CHANGE_SHOWN;
                _moneyChange = (isFresh ? _moneyChange : 0) + money - _money;
                _materialsChange = (isFresh ? _materialsChange : 0) + materials - _materials;
                _walletChangedAt = Time.time;
            }

            _hasWallet = true;
            _money = money;
            _carried = carried;
            _materials = materials;
            DrawWallet();
        }

        public void ShowProgress(int level, int intoLevel, int levelSize)
        {
            _progress = $"\n<size=70%><color=#9AD0FF>{Localizer.Format("Seviye {0}", level)}  ({intoLevel}/{levelSize} XP)</color></size>";
            DrawWallet();
        }

        private void DrawWallet()
        {
            _walletText.gameObject.SetActive(true);
            bool showsChange = Time.time - _walletChangedAt < WALLET_CHANGE_SHOWN;
            string money = showsChange ? Change(_moneyChange) : string.Empty;
            string materials = showsChange ? Change(_materialsChange) : string.Empty;
            _walletText.text = $"<color=#FFC857>{Localizer.Format("Para: {0}", _money)}</color>{money}  <color=#C8B08A>{Localizer.Format("Malzeme: {0}", _materials)}</color>{materials}\n<size=80%>{Localizer.Format("Çanta: {0}", _carried)}</size>{_progress}";
        }

        private static string Change(int amount) => amount switch
        {
            > 0 => $" <size=80%><color=#7CFF7C>+{amount}</color></size>",
            < 0 => $" <size=80%><color=#FF6B6B>{amount}</color></size>",
            _ => string.Empty
        };

        private bool _walletChangeDrawn;

        private void DrawWalletPop()
        {
            if (_walletText == null)
                return;

            float age = Time.time - _walletChangedAt;
            float pop = Mathf.Clamp01(1f - age / WALLET_POP);
            _walletText.rectTransform.localScale = Vector3.one * (1f + 0.15f * pop * pop);

            // The change beside the totals goes once its moment is over.
            bool showsChange = age < WALLET_CHANGE_SHOWN;
            if (showsChange == _walletChangeDrawn)
                return;

            _walletChangeDrawn = showsChange;
            DrawWallet();
        }

        public void ShowObjective(string title, bool isActive, float progress)
        {
            _objective = isActive ? $"\n<size=55%><color=#7FE08A>{Localizer.Format("GÖREV: {0}", L(title))}  %{Mathf.RoundToInt(progress * 100f)}</color></size>" : string.Empty;
            DrawWave();
        }

        public void ShowObjectiveEnded(bool isHeld, int reward) => ShowBanner(isHeld
            ? $"<color=#7FE08A>{L("GÖREV TAMAM")}</color>\n<size=60%><color=#FFC857>{Localizer.Format("+{0} para", reward)}</color></size>"
            : $"<color=#FF5A5A>{L("GÖREV BAŞARISIZ")}</color>");

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
            _reviveLabel.text = isReviving ? "..." : L("CANLANDIR");
        }

        private string _weaponName;
        private int _ammo;
        private int _magazineSize;
        private int _reserve = -1;

        public void ShowAmmo(int ammo, int magazineSize, int reserve)
        {
            _ammo = ammo;
            _magazineSize = magazineSize;
            _reserve = reserve;
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

            // Spare rounds beside the magazine; red once they are gone, a sign that never runs out on the fallback weapon.
            string spare = _reserve < 0 ? "∞" : _reserve == 0 ? "<color=#FF5A5A>0</color>" : _reserve.ToString();
            string ammo = $"{_ammo} / {_magazineSize}  <size=70%><color=#9AA4B2>{spare}</color></size>";
            _ammoText.text = string.IsNullOrEmpty(_weaponName) ? ammo : $"<size=60%>{L(_weaponName)}</size>\n{ammo}";
        }

        public void ShowHurt(float amount)
        {
            _hurtAlpha = Mathf.Clamp(_hurtAlpha + 0.25f + amount / 60f, 0f, 0.85f);
        }

        public void ShowWaveCleared(int wave, int reward) =>
            ShowBanner($"{Localizer.Format("DALGA {0} TEMİZLENDİ", wave)}\n<size=60%><color=#FFC857>{Localizer.Format("+{0} para", reward)}</color></size>");

        /// <summary>The boss's health across the top; a boss that just came in is announced.</summary>
        public void ShowBoss(string name, int health, int maxHealth)
        {
            bool isShown = !string.IsNullOrEmpty(name) && maxHealth > 0;
            if (isShown && !_bossBar.gameObject.activeSelf)
                ShowBanner($"<color=#FF5A5A>{L("BOSS GELİYOR")}</color>\n<size=60%>{L(name)}</size>");

            _bossBar.gameObject.SetActive(isShown);
            if (!isShown)
                return;

            _bossFill.anchorMax = new Vector2(Mathf.Clamp01((float) health / maxHealth), 1f);
            _bossLabel.text = $"{L(name)}  {health} / {maxHealth}";
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
            int seconds = _waveStartTime > 0f ? Mathf.CeilToInt(_waveStartTime - Time.time) : 0;
            _skipWaveButton.gameObject.SetActive(_wave > 0 && seconds > 0);
            if (_wave <= 0)
                return;

            _waveText.text = seconds > 0
                ? $"{Localizer.Format("DALGA {0}", _wave)}\n<size=70%>{Localizer.Format("{0} sn sonra geliyor", seconds)}</size>"
                : $"{Localizer.Format("DALGA {0}", _wave)}\n<size=70%>{Localizer.Format("Kalan zombi: {0}", _zombiesLeft)}</size>{_objective}";
        }

        protected override void PlayShowAnimation() => ShowCompleted?.Invoke(this);

        protected override void PlayHideAnimation() => HideCompleted?.Invoke(this);
    }
}
