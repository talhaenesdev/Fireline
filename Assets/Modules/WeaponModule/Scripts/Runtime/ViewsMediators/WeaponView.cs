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
        [SerializeField] private CD_Weapon _config;
        [SerializeField] private Transform _muzzlePoint;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _fireTrigger = "Fire";

        private readonly List<ulong> _otherClients = new();

        public bool IsRegistered { get; set; }

        public Action Spawned;
        public Action Despawned;
        public Action<Vector3, Vector3> ServerFireRequested;
        public Action<Vector3> RemoteFired;

        public CD_Weapon Config => _config;
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
            Spawned?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            Despawned?.Invoke();
            base.OnNetworkDespawn();
        }

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
