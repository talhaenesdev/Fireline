using FireLine.Scripts.Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace FireLine.Scripts.UI.Controller
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField]
        private Image healthFill;

        [SerializeField]
        private TMP_Text healthText;

        private NetworkPlayerHealth _health;

        private void Update()
        {
            if (_health != null)
                return;

            TryFindLocalPlayer();
        }

        private void TryFindLocalPlayer()
        {
            if (NetworkManager.Singleton == null)
                return;

            if (!NetworkManager.Singleton.IsClient)
                return;

            NetworkPlayerHealth[] players =
                FindObjectsByType<NetworkPlayerHealth>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None
                );

            foreach (NetworkPlayerHealth player in players)
            {
                if (player == null)
                    continue;

                if (!player.IsOwner)
                    continue;

                Initialize(player);

                break;
            }
        }

        private void Initialize(
            NetworkPlayerHealth health)
        {
            if (_health != null)
            {
                _health.HealthChanged -=
                    OnHealthChanged;
            }

            _health = health;

            if (_health == null)
            {
                Debug.LogError(
                    "[HEALTH UI] " +
                    "NetworkPlayerHealth is NULL!"
                );

                return;
            }

            _health.HealthChanged +=
                OnHealthChanged;

            UpdateUI(
                _health.CurrentHealth,
                _health.MaxHealth
            );

            Debug.Log(
                $"[HEALTH UI] Initialized | " +
                $"Owner={_health.OwnerClientId} | " +
                $"Health={_health.CurrentHealth}/" +
                $"{_health.MaxHealth}"
            );
        }

        private void OnHealthChanged(
            float previous,
            float current)
        {
            if (_health == null)
                return;

            UpdateUI(
                current,
                _health.MaxHealth
            );
        }

        private void UpdateUI(
            float currentHealth,
            float maxHealth)
        {
            if (maxHealth <= 0f)
                return;

            float fillAmount =
                currentHealth / maxHealth;

            if (healthFill != null)
            {
                healthFill.fillAmount =
                    fillAmount;
            }

            if (healthText != null)
            {
                healthText.text =
                    $"{Mathf.CeilToInt(currentHealth)} / " +
                    $"{Mathf.CeilToInt(maxHealth)}";
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.HealthChanged -=
                    OnHealthChanged;
            }
        }
    }
}