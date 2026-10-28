using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.MatchModule.Signals;

namespace Modules.MatchModule.ViewsMediators
{
    public class ScoreboardNetworkMediator : IMediator
    {
        [Inject] private ScoreboardNetworkView _view { get; set; }
        [InjectSignal] private MatchSignals _signals { get; set; }
        [InjectSignal] private MatchInternalSignals _internalSignals { get; set; }

        public void OnRegister()
        {
            _view.Spawned += OnSpawned;
            _view.EntriesChanged += OnEntriesChanged;
            _view.GameOver += OnGameOver;

            if (_view.IsSpawned)
            {
                OnSpawned();
                OnEntriesChanged();
            }
        }

        public void OnRemove()
        {
            _view.Spawned -= OnSpawned;
            _view.EntriesChanged -= OnEntriesChanged;
            _view.GameOver -= OnGameOver;
        }

        private void OnSpawned() => _internalSignals.ScoreboardRegistered.Dispatch(_view);

        private void OnEntriesChanged() => _signals.Outgoing.ScoresChanged.Dispatch(_view.ToEntries());

        private void OnGameOver() => _signals.Outgoing.GameOver.Dispatch();
    }
}
