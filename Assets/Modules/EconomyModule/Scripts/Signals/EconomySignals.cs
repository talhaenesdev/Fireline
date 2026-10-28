using FlowIoC.BaseModule.Signals;
using Modules.EconomyModule.Shared.Data.ValueObjects;
using UnityEngine;

namespace Modules.EconomyModule.Signals
{
    public class EconomySignals : ISignalHolder
    {
        public EconomySignalsIncoming Incoming = new();
        public EconomySignalsOutgoing Outgoing = new();
    }

    public class EconomySignalsIncoming
    {
        /// <summary>A zombie fell here; it may leave loot. Server only.</summary>
        public Signal<Vector3> DropLoot = new();

        /// <summary>The local player wants this item of the shop.</summary>
        public Signal<int> Buy = new();
    }

    public class EconomySignalsOutgoing
    {
        /// <summary>The local player's money, then how many pieces of loot it carries.</summary>
        public Signal<int, int> LocalWalletChanged = new();

        /// <summary>The local player walked into the shop zone: what the shop sells.</summary>
        public Signal<ShopItemCVO[]> ShopEntered = new();

        /// <summary>The local player walked out of the shop zone.</summary>
        public Signal ShopLeft = new();

        /// <summary>A player bought health: client id, points. Server only.</summary>
        public Signal<ulong, float> HealPurchased = new();

        /// <summary>A player bought a shield: client id, points. Server only.</summary>
        public Signal<ulong, float> ShieldPurchased = new();

        /// <summary>The local player bought a faster trigger: how much shorter each shot's wait gets.</summary>
        public Signal<float> FireRatePurchased = new();
    }
}
