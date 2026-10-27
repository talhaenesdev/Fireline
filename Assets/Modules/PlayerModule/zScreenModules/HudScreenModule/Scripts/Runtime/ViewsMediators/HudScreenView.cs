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
        [SerializeField] private Image _healthFill;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TMP_Text _ammoText;

        public Action<Vector2> TouchMoved;
        public Action<bool, bool> TouchFireChanged;
        public Action TouchReloadPressed;

        private TouchJoystick _stick;
        private TouchHoldButton _fire;
        private Button _reload;

        // A phone has no keyboard or mouse; it plays from controls drawn on the HUD.
        private void Awake()
        {
            if (!Application.isMobilePlatform)
                return;

            _stick = TouchControlsBuilder.Joystick(transform);
            _fire = TouchControlsBuilder.HoldButton(transform, "FireButton", new Vector2(-150f, 150f), 150f, new Color(0.85f, 0.25f, 0.2f, 0.55f));
            _reload = TouchControlsBuilder.TapButton(transform, "ReloadButton", new Vector2(-290f, 110f), 90f, new Color(1f, 1f, 1f, 0.35f));
        }

        private void OnEnable()
        {
            if (_stick == null)
                return;

            _stick.Moved += OnStickMoved;
            _fire.HeldChanged += OnFireHeld;
            _reload.onClick.AddListener(OnReloadClicked);
        }

        private void OnDisable()
        {
            if (_stick == null)
                return;

            _stick.Moved -= OnStickMoved;
            _fire.HeldChanged -= OnFireHeld;
            _reload.onClick.RemoveListener(OnReloadClicked);
        }

        private void OnStickMoved(Vector2 move) => TouchMoved?.Invoke(move);
        private void OnFireHeld(bool isHeld) => TouchFireChanged?.Invoke(isHeld, isHeld);
        private void OnReloadClicked() => TouchReloadPressed?.Invoke();

        public void ShowHealth(float health, float maxHealth)
        {
            if (maxHealth <= 0f)
                return;

            if (_healthFill != null)
                _healthFill.fillAmount = health / maxHealth;

            if (_healthText != null)
                _healthText.text = $"{Mathf.CeilToInt(health)} / {Mathf.CeilToInt(maxHealth)}";
        }

        public void ShowAmmo(int ammo, int magazineSize)
        {
            if (_ammoText != null)
                _ammoText.text = $"{ammo} / {magazineSize}";
        }

        protected override void PlayShowAnimation() => ShowCompleted?.Invoke(this);

        protected override void PlayHideAnimation() => HideCompleted?.Invoke(this);
    }
}
