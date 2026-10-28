using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using Modules.EconomyModule.Entities;
using Modules.EconomyModule.Enums;
using UnityEngine;

namespace Modules.EconomyModule.ViewsMediators
{
    /// <summary>
    /// The sell zone or the shop zone, placed in the scene under EconomySystemRoot. Move it and
    /// change its radius in the scene; standing within the radius is standing in the zone.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class EconomyZoneView : MonoBehaviour, IView, IEconomyZone
    {
        [SerializeField] private EconomyZoneKind _kind;
        [SerializeField] private float _radius = 3f;

        public bool IsRegistered { get; set; }

        public EconomyZoneKind Kind => _kind;
        public Vector3 Position => transform.position;
        public float Radius => _radius;

        private void OnDrawGizmos()
        {
            Gizmos.color = _kind == EconomyZoneKind.Sell ? new Color(1f, 0.8f, 0.2f) : new Color(0.3f, 0.7f, 1f);
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
