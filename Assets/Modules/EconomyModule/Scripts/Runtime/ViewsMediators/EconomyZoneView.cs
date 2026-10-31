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
        [Tooltip("Unique among the scene's zones; a purchase names the zone by it.")]
        [SerializeField] private int _id;
        [SerializeField] private EconomyZoneKind _kind;
        [Tooltip("A shop zone or perk machine's own stock; empty uses the board's shop.")]
        [SerializeField] private Data.UnityObjects.CD_Shop _shop;
        [SerializeField] private float _radius = 3f;
        [Tooltip("A door: what stands in the way until it is bought open.")]
        [SerializeField] private GameObject _blocker;

        public bool IsRegistered { get; set; }

        public int Id => _id;
        public EconomyZoneKind Kind => _kind;
        public Data.UnityObjects.CD_Shop Shop => _shop;
        public Vector3 Position => transform.position;
        public float Radius => _radius;

        public void ShowOpen(bool isOpen)
        {
            if (_blocker != null)
                _blocker.SetActive(!isOpen);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _kind == EconomyZoneKind.Sell ? new Color(1f, 0.8f, 0.2f) : _kind == EconomyZoneKind.PerkMachine ? new Color(0.8f, 0.3f, 1f) : new Color(0.3f, 0.7f, 1f);
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
