using FlowIoC.BaseModule.Signals;
using Modules.EconomyModule.Shared.Data.ValueObjects;

namespace Modules.EconomyModule.ShopScreenModule.Signals
{
    public class ShopScreenSignals : ISignalHolder
    {
        public ShopScreenSignalsIncoming Incoming = new();
        public ShopScreenSignalsOutgoing Outgoing = new();
    }

    public class ShopScreenSignalsIncoming
    {
        /// <summary>Open the shop with what it sells.</summary>
        public Signal<ShopItemCVO[]> Open = new();

        public Signal Close = new();

        /// <summary>The local player's money, so the shop shows what is affordable.</summary>
        public Signal<int> ApplyMoney = new();
    }

    public class ShopScreenSignalsOutgoing
    {
        /// <summary>The player pressed buy on this item, by its place in the list.</summary>
        public Signal<int> BuyClicked = new();
    }
}
