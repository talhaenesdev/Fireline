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

        /// <summary>A zombie fell to this player: client id. Server only.</summary>
        public Signal<ulong> CreditKill = new();

        /// <summary>Every player is paid this much, for an objective held. Server only.</summary>
        public Signal<int> PayEveryone = new();

        /// <summary>A wave was cleared: its number. On every client.</summary>
        public Signal<int> WaveCleared = new();

        /// <summary>The local player wants this item of the shop.</summary>
        public Signal<int> Buy = new();
    }

    public class EconomySignalsOutgoing
    {
        /// <summary>The local player's money, then how many pieces of loot it carries.</summary>
        public Signal<int, int> LocalWalletChanged = new();

        /// <summary>The local player's level, the experience into it, and how much the level takes.</summary>
        public Signal<int, int, int> LocalProgressChanged = new();

        /// <summary>A cleared wave paid every player: the wave, then the money each got.</summary>
        public Signal<int, int> WaveRewardAnnounced = new();

        /// <summary>The local player walked into the shop zone: what the shop sells.</summary>
        public Signal<ShopItemCVO[]> ShopEntered = new();

        /// <summary>The local player walked out of the shop zone.</summary>
        public Signal ShopLeft = new();

        /// <summary>A player bought health: client id, points. Server only.</summary>
        public Signal<ulong, float> HealPurchased = new();

        /// <summary>A player bought a shield: client id, points. Server only.</summary>
        public Signal<ulong, float> ShieldPurchased = new();

        /// <summary>A player bought a turret: client id. Server only.</summary>
        public Signal<ulong> TurretPurchased = new();

        /// <summary>A player bought a weapon: client id, its index in the weapon set. Server only.</summary>
        public Signal<ulong, int> WeaponPurchased = new();

        /// <summary>A player bought a turret upgrade: client id. Server only.</summary>
        public Signal<ulong> TurretUpgradePurchased = new();

        /// <summary>A player bought a barricade: client id. Server only.</summary>
        public Signal<ulong> BarricadePurchased = new();

        /// <summary>A player bought a wall: client id. Server only.</summary>
        public Signal<ulong> WallPurchased = new();

        /// <summary>A player bought a barracks: client id. Server only.</summary>
        public Signal<ulong> BarracksPurchased = new();

        /// <summary>A player bought a perk: client id, which perk, how strong. Server only.</summary>
        public Signal<ulong, int, float> PerkPurchased = new();

        /// <summary>The local player bought faster reloads: the share of the time taken off.</summary>
        public Signal<float> ReloadPerkPurchased = new();

        /// <summary>The local player bought a faster trigger: how much shorter each shot's wait gets.</summary>
        public Signal<float> FireRatePurchased = new();
    }
}
