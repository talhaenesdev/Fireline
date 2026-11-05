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
        private const float HIT_RADIUS = 0.06f;

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

            // Swept, not left to the trigger: a fast round moves further in one step than a small
            // zombie is wide, and a trigger only sees where it lands, so it flew through them.
            float distance = _speed * Time.deltaTime;
            Vector3 start = transform.position;
            RaycastHit[] hits = Physics.SphereCastAll(start, HIT_RADIUS, _direction, distance,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == null || hit.collider.transform.IsChildOf(transform))
                    continue;

                if (hit.collider.GetComponentInParent<NetworkBulletView>() != null)
                    continue;

                // Where the round met it, so a wall shows its impact there.
                transform.position = start + _direction * hit.distance;
                Hit?.Invoke(hit.collider);

                if (!IsSpawned)
                    return;
            }

            transform.position = start + _direction * distance;
            _remainingLifetime -= Time.deltaTime;

            if (_remainingLifetime <= 0f)
                Despawn();
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
