using FlowIoC.BaseModule.Connectors;
using FlowIoC.BaseModule.Contexts;
using Modules.LoadingModule.LoadingScreenModule.Signals;
using Modules.MainModule.MainScreenModule.Signals;
using Modules.MainModule.Signals;

namespace Modules.ConnectorModule.RootsContexts
{
    public class MainConnectorSubContext : Context
    {
        private const string GROUP = nameof(MainConnectorSubContext);

        private MainSignals _mainSignals;
        private MainScreenSignals _mainScreenSignals;
        private LoadingScreenSignals _loadingScreenSignals;

        public override void Setup()
        {
            base.Setup();

            _mainSignals = InjectionBinderCrossContext.GetInstance<MainSignals>();
            _mainScreenSignals = InjectionBinderCrossContext.GetInstance<MainScreenSignals>();
            _loadingScreenSignals = InjectionBinderCrossContext.GetInstance<LoadingScreenSignals>();

            IncomingSignals();
            OutgoingSignals();
        }

        private void IncomingSignals()
        {
            _mainSignals.Outgoing.Started.Connect(_mainScreenSignals.Incoming.OpenMainScreen, GROUP);

            // The loading screen's retry button asks Main to boot again; the set it names is Main's boot.
            _loadingScreenSignals.Outgoing.RetryClicked.Connect(_ => _mainSignals.Incoming.RetryBoot.Dispatch(), GROUP);
        }

        private void OutgoingSignals()
        {
            _mainScreenSignals.Outgoing.QuitClicked.Connect(_mainSignals.Incoming.Quit, GROUP);
        }

        public override void DestroyContext()
        {
            SignalConnector.DisconnectGroup(GROUP);
            base.DestroyContext();
        }
    }
}
