using System;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using Modules.PlayerModule.Entities;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Modules.PlayerModule.ViewsMediators
{
    /// <summary>
    /// The networked player: its replicated name and health, and how death looks. A prefab spawned
    /// by Netcode sits under no Root, so its ViewInjector names PlayerSystemRoot.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class PlayerAvatarView : NetworkBehaviour, IView, IPlayerAvatar
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 8f;

        [Header("Health")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private ParticleSystem _hitParticle;
        [SerializeField] private ParticleSystem _deathParticle;

        private readonly NetworkVariable<FixedString64Bytes> _playerName = new(default,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<float> _health = new(100f,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<bool> _isDead = new(false,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private Collider[] _colliders;
        private AudioListener _audioListener;

        public bool IsRegistered { get; set; }

        public Action Spawned;
        public Action Despawned;
        public Action<string> NameRequested;
        public Action<string> NameChanged;
        public Action<float, float> HealthChanged;
        public Action<bool> DeathChanged;

        public ulong ClientId => OwnerClientId;
        public Transform Transform => transform;
        public string PlayerName => _playerName.Value.ToString();
        public float MoveSpeed => _moveSpeed;
        public float SprintSpeed => _sprintSpeed;
        public float Health => _health.Value;
        public float MaxHealth => _maxHealth;
        public bool IsDead => _isDead.Value;

        private void Awake()
        {
            _colliders = GetComponentsInChildren<Collider>(true);
            _audioListener = GetComponentInChildren<AudioListener>(true);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _playerName.OnValueChanged += OnNameChanged;
            _health.OnValueChanged += OnHealthChanged;
            _isDead.OnValueChanged += OnDeathChanged;

            if (IsServer)
            {
                _health.Value = _maxHealth;
                _isDead.Value = false;
            }

            // Only the owner hears the world from this avatar.
            if (_audioListener != null)
                _audioListener.enabled = IsOwner;

            ShowDeath(_isDead.Value);
            Spawned?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            _playerName.OnValueChanged -= OnNameChanged;
            _health.OnValueChanged -= OnHealthChanged;
            _isDead.OnValueChanged -= OnDeathChanged;

            Despawned?.Invoke();
            base.OnNetworkDespawn();
        }

        public void RequestName(string playerName)
        {
            if (IsOwner)
                RequestNameServerRpc(playerName);
        }

        public void SetNameServer(string playerName)
        {
            if (IsServer)
                _playerName.Value = new FixedString64Bytes(playerName);
        }

        public void SetHealthServer(float health)
        {
            if (IsServer)
                _health.Value = health;
        }

        public void SetDeadServer(bool isDead)
        {
            if (IsServer)
                _isDead.Value = isDead;
        }

        public void PlayHitEffectServer()
        {
            if (IsServer)
                PlayHitEffectClientRpc();
        }

        public void ShowDeath(bool isDead)
        {
            if (_colliders != null)
            {
                foreach (Collider target in _colliders)
                {
                    if (target != null)
                        target.enabled = !isDead;
                }
            }

            if (_deathParticle == null)
                return;

            if (isDead)
            {
                _deathParticle.transform.position = transform.position;
                _deathParticle.Play();
            }
            else
            {
                _deathParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        [ServerRpc]
        private void RequestNameServerRpc(string playerName) => NameRequested?.Invoke(playerName);

        [ClientRpc]
        private void PlayHitEffectClientRpc()
        {
            if (_hitParticle != null)
                _hitParticle.Play();
        }

        private void OnNameChanged(FixedString64Bytes previous, FixedString64Bytes current) =>
            NameChanged?.Invoke(current.ToString());

        private void OnHealthChanged(float previous, float current) => HealthChanged?.Invoke(current, _maxHealth);

        private void OnDeathChanged(bool previous, bool current) => DeathChanged?.Invoke(current);
    }
}
