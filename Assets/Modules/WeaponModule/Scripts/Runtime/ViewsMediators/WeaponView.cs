using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using Modules.WeaponModule.Data.UnityObjects;
using Modules.WeaponModule.Entities;
using Unity.Netcode;
using UnityEngine;

namespace Modules.WeaponModule.ViewsMediators
{
    /// <summary>
    /// The weapon a player carries: its muzzle, its flash, and the two calls that carry a shot over
    /// the network. It sits on the player prefab, so its ViewInjector names WeaponSystemRoot.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class WeaponView : NetworkBehaviour, IView, IWeapon
    {
        [Tooltip("The weapon a player without a set carries.")]
        [SerializeField] private CD_Weapon _config;
        [Tooltip("Every weapon a player can be given; element 0 is the one they start with.")]
        [SerializeField] private CD_WeaponSet _weaponSet;
        [SerializeField] private Transform _muzzlePoint;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _fireTrigger = "Fire";

        private readonly List<ulong> _otherClients = new();

        private readonly NetworkVariable<int> _weaponIndex = new(0,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public bool IsRegistered { get; set; }

        public Action Spawned;
        public Action Despawned;
        public Action<Vector3, Vector3> ServerFireRequested;
        public Action<Vector3> RemoteFired;
        public Action WeaponChanged;

        public CD_Weapon Config => _weaponSet != null && _weaponSet.Get(_weaponIndex.Value) != null ? _weaponSet.Get(_weaponIndex.Value) : _config;
        public int WeaponIndex => _weaponIndex.Value;
        public Transform Owner => transform;
        public Vector3 MuzzlePosition => _muzzlePoint != null ? _muzzlePoint.position : transform.position;

        private void Awake()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _weaponIndex.OnValueChanged += OnWeaponIndexChanged;
            Spawned?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            _weaponIndex.OnValueChanged -= OnWeaponIndexChanged;
            Despawned?.Invoke();
            base.OnNetworkDespawn();
        }

        public void SetWeaponServer(int index)
        {
            if (IsServer && _weaponSet != null && _weaponSet.Get(index) != null)
                _weaponIndex.Value = index;
        }

        private void OnWeaponIndexChanged(int previous, int current) => WeaponChanged?.Invoke();

        public void RequestServerFire(Vector3 position, Vector3 direction)
        {
            if (IsOwner)
                RequestFireServerRpc(position, direction);
        }

        public void BroadcastFire(Vector3 position)
        {
            if (!IsServer)
                return;

            _otherClients.Clear();
            foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
            {
                if (clientId != OwnerClientId)
                    _otherClients.Add(clientId);
            }

            if (_otherClients.Count == 0)
                return;

            PlayRemoteFireClientRpc(position, new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = _otherClients.ToArray() }
            });
        }

        public void ShowFire()
        {
            if (_muzzleFlash != null)
            {
                _muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _muzzleFlash.Play();
            }

            if (_animator != null)
                _animator.SetTrigger(_fireTrigger);
        }

        [ServerRpc]
        private void RequestFireServerRpc(Vector3 position, Vector3 direction) =>
            ServerFireRequested?.Invoke(position, direction);

        [ClientRpc]
        private void PlayRemoteFireClientRpc(Vector3 position, ClientRpcParams clientRpcParams = default) =>
            RemoteFired?.Invoke(position);
    }
}
