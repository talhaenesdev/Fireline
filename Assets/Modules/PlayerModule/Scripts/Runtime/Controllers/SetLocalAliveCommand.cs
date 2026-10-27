using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>A freshly spawned local avatar is alive, whatever the last one was.</summary>
    internal class SetLocalAliveCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }

        public override void Execute()
        {
            _model.IsLocalAlive = _model.LocalAvatar != null && !_model.LocalAvatar.IsDead;
            _signals.Outgoing.LocalAliveChanged.Dispatch(_model.IsLocalAlive);
            _signals.Outgoing.LocalHealthChanged.Dispatch(_model.LocalAvatar?.Health ?? 0f, _model.LocalAvatar?.MaxHealth ?? 0f);
        }
    }
}
