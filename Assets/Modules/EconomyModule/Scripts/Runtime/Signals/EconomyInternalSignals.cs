using FlowIoC.BaseModule.Signals;
using Modules.EconomyModule.Entities;

namespace Modules.EconomyModule.Signals
{
    /// <summary>What the module says to its own commands.</summary>
    internal class EconomyInternalSignals : ISignalHolder
    {
        /// <summary>The module is up: the economy's frame starts.</summary>
        public Signal Started = new();

        public Signal<IEconomyBoard> BoardRegistered = new();
        public Signal<IEconomyZone> ZoneRegistered = new();

        /// <summary>A wallet changed on the replicated board.</summary>
        public Signal WalletsChanged = new();

        /// <summary>Server: a client asked to buy: client id, item.</summary>
        public Signal<ulong, int> BuyRequested = new();

        /// <summary>Buyer's process: the server sold it this item.</summary>
        public Signal<int> Purchased = new();

        /// <summary>The economy's frame: pick up, sell, and the shop zone.</summary>
        public Signal Tick = new(hideCommandLog: true);
    }
}
