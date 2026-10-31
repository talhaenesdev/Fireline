using FlowIoC.BaseModule.Contexts;
using Modules.EconomyModule.Controllers;
using Modules.EconomyModule.Models;
using Modules.EconomyModule.Signals;
using Modules.EconomyModule.ViewsMediators;

namespace Modules.EconomyModule.RootsContexts
{
    public class EconomySystemContext : Context
    {
        private EconomySignals _signals;
        private EconomyInternalSignals _internalSignals;
        private IEconomyModel _model;

        public override void SignalBindings()
        {
            base.SignalBindings();
            _internalSignals = InjectionBinder.Bind<EconomyInternalSignals>();
            _signals = InjectionBinderCrossContext.Bind<EconomySignals>();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();
            _model = InjectionBinder.Bind<IEconomyModel, EconomyModel>();
        }

        public override void MediationBindings()
        {
            base.MediationBindings();
            MediationBinder.Bind<EconomyNetworkView>().To<EconomyNetworkMediator>();
            MediationBinder.Bind<EconomyZoneView>().To<EconomyZoneMediator>();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();

            CommandBinder.Bind(_internalSignals.Started).ToSequence<StartFrameTickCommand>();
            CommandBinder.Bind(_internalSignals.BoardRegistered).ToSequence<StoreBoardCommand>();
            CommandBinder.Bind(_internalSignals.ZoneRegistered).ToSequence<StoreZoneCommand>();
            CommandBinder.Bind(_internalSignals.WalletsChanged)
                .ToSequence<PublishLocalWalletCommand>()
                .ToSequence<SaveLocalXpCommand>();

            CommandBinder.Bind(_internalSignals.DoorsChanged).ToSequence<ApplyDoorsCommand>();

            CommandBinder.Bind(_internalSignals.LevelReported)
                .ToSequence<RequireServerCommand>()
                .ToSequence<PayForLevelCommand>();

            CommandBinder.Bind(_signals.Incoming.CreditKill)
                .ToSequence<RequireServerCommand>()
                .ToSequence<CreditKillCommand>();

            CommandBinder.Bind(_signals.Incoming.PayEveryone)
                .ToSequence<RequireServerCommand>()
                .ToSequence<PayEveryoneCommand>();
            CommandBinder.Bind(_internalSignals.Purchased).ToSequence<ApplyPurchaseCommand>();

            CommandBinder.Bind(_internalSignals.BuyRequested)
                .ToSequence<RequireServerCommand>()
                .ToSequence<SellItemCommand>();

            CommandBinder.Bind(_signals.Incoming.DropLoot)
                .ToSequence<RequireServerCommand>()
                .ToSequence<DropLootCommand>();

            CommandBinder.Bind(_signals.Incoming.Buy).ToSequence<RequestBuyCommand>();

            CommandBinder.Bind(_signals.Incoming.WaveCleared)
                .ToSequence<AnnounceWaveRewardCommand>()
                .ToSequence<RequireServerCommand>()
                .ToSequence<GrantWaveRewardCommand>()
                .ToSequence<CreditWaveCommand>();

            // Every process watches its own player against the shop; the server also hands out loot and money.
            CommandBinder.Bind(_internalSignals.Tick)
                .ToSequence<CheckShopZoneCommand>()
                .ToSequence<RequireServerCommand>()
                .ToSequence<CollectLootCommand>()
                .ToSequence<SellLootCommand>();
        }

        public override void Launch()
        {
            base.Launch();
            _internalSignals.Started.Dispatch();
        }

        public override void DestroyContext()
        {
            // The update loop outlives the scene; the flag lets its callback take itself off.
            if (_model != null)
                _model.IsTicking = false;

            base.DestroyContext();
        }
    }
}
