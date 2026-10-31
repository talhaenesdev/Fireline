using System;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using Modules.WeaponModule.Entities;
using Unity.Netcode;
using UnityEngine;

namespace Modules.WeaponModule.ViewsMediators
{
    /// <summary>
    /// The authoritative round. The server flies it and reports what it touched; the shooter does
    /// not draw it, because the shooter already sees its own predicted round.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class NetworkBulletView : NetworkBehaviour, IView, INetworkBullet
    {
        private Vector3 _direction;
        private float _speed;
        private float _remainingLifetime;

        public bool IsRegistered { get; set; }

        public Action<Collider> Hit;
        public Action<Vector3> ImpactShown;

        public ulong ShooterClientId { get; private set; }
        public int Damage { get; private set; }
        public Vector3 Position => transform.position;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // The shooter sees its own predicted round instead. Decided on every spawn: a round from
            // the pool may have been somebody else's last time.
            foreach (Renderer bulletRenderer in GetComponentsInChildren<Renderer>(true))
                bulletRenderer.enabled = !IsOwner;
        }

        public void Launch(Vector3 direction, float speed, float lifetime, int damage, ulong shooterClientId)
        {
            if (!IsServer)
                return;

            _direction = direction.normalized;
            _speed = speed;
            _remainingLifetime = lifetime;
            Damage = damage;
            ShooterClientId = shooterClientId;
        }

        private void Update()
        {
            if (!IsServer || !IsSpawned)
                return;

            transform.position += _direction * (_speed * Time.deltaTime);
            _remainingLifetime -= Time.deltaTime;

            if (_remainingLifetime <= 0f)
                Despawn();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsServer && IsSpawned)
                Hit?.Invoke(other);
        }

        public void ShowImpact(Vector3 point)
        {
            if (IsServer)
                ShowImpactClientRpc(point);
        }

        public void Despawn()
        {
            if (IsServer && NetworkObject != null && NetworkObject.IsSpawned)
                NetworkObject.Despawn();
        }

        [ClientRpc]
        private void ShowImpactClientRpc(Vector3 point) => ImpactShown?.Invoke(point);
    }
}
