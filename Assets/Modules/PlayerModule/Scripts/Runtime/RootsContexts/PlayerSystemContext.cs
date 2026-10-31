using FlowIoC.BaseModule.Contexts;
using FlowIoC.BaseModule.Controller.Commands;
using Modules.PlayerModule.Controllers;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;
using Modules.PlayerModule.ViewsMediators;

namespace Modules.PlayerModule.RootsContexts
{
    public class PlayerSystemContext : Context
    {
        private PlayerSignals _signals;
        private PlayerInternalSignals _internalSignals;
        private IPlayerModel _model;

        public override void SignalBindings()
        {
            base.SignalBindings();
            _internalSignals = InjectionBinder.Bind<PlayerInternalSignals>();
            _signals = InjectionBinderCrossContext.Bind<PlayerSignals>();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();
            _model = InjectionBinder.Bind<IPlayerModel, PlayerModel>();
        }

        public override void MediationBindings()
        {
            base.MediationBindings();
            MediationBinder.Bind<PlayerAvatarView>().To<PlayerAvatarMediator>();
            MediationBinder.Bind<PlayerInputView>().To<PlayerInputMediator>();
            MediationBinder.Bind<PlayerMotorView>().To<PlayerMotorMediator>();
            MediationBinder.Bind<PlayerAimView>().To<PlayerAimMediator>();
            MediationBinder.Bind<PlayerCameraView>().To<PlayerCameraMediator>();
            MediationBinder.Bind<PlayerVisionView>().To<PlayerVisionMediator>();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();

            // Every avatar is registered; the local one also brings its name, its frame and its life.
            CommandBinder.Bind(_internalSignals.AvatarSpawned)
                .ToSequence<RegisterAvatarCommand>()
                .ToSequence<RequireLocalAvatarCommand>()
                .ToSequence<RequestPlayerNameCommand>()
                .ToSequence<DetectTouchInputCommand>()
                .ToSequence<StartFrameTickCommand>()
                .ToSequence<SetLocalAliveCommand>();

            CommandBinder.Bind(_internalSignals.AvatarDespawned).ToSequence<UnregisterAvatarCommand>();

            CommandBinder.Bind(_internalSignals.NameRequested).ToSequence<ApplyPlayerNameCommand>();
            CommandBinder.Bind(_internalSignals.NameChanged).ToSequence<PublishPlayerNameCommand>();
            CommandBinder.Bind(_internalSignals.HealthChanged)
                .ToSequence<PublishLocalHealthCommand>()
                .ToSequence<DetectLocalHurtCommand>();
            CommandBinder.Bind(_internalSignals.DeathChanged).ToSequence<ApplyLocalDeathCommand>();
            CommandBinder.Bind(_internalSignals.ShieldChanged).ToSequence<PublishLocalShieldCommand>();
            CommandBinder.Bind(_internalSignals.ReviveRequested).ToSequence<BeginServerReviveCommand>();

            CommandBinder.Bind(_internalSignals.MoveInput).ToSequence<StoreMoveInputCommand>();
            CommandBinder.Bind(_internalSignals.SprintInput).ToSequence<StoreSprintInputCommand>();

            CommandBinder.Bind(_internalSignals.Tick)
                .ToSequence<MoveLocalAvatarCommand>()
                .ToSequence<AutoAimCommand>()
                .ToSequence<TrackReviveCommand>()
                .ToSequence<ResolveRevivesCommand>()
                .ToSequence<CollectMarkersCommand>();

            CommandBinder.Bind(_signals.Incoming.ApplyPerk).ToSequence<ApplyPerkCommand>();
            CommandBinder.Bind(_signals.Incoming.Ping).ToSequence<RequestPingCommand>();
            CommandBinder.Bind(_internalSignals.PingPressed).ToSequence<RequestPingCommand>();
            CommandBinder.Bind(_internalSignals.AimPointChanged).ToSequence<StoreAimPointCommand>();
            CommandBinder.Bind(_internalSignals.PingRequested).ToSequence<BroadcastPingCommand>();
            CommandBinder.Bind(_internalSignals.PingShown).ToSequence<ShowPingCommand>();

            CommandBinder.Bind(_signals.Incoming.ApplyDamage).ToSequence<ApplyDamageCommand>();
            CommandBinder.Bind(_signals.Incoming.Heal).ToSequence<HealCommand>();
            CommandBinder.Bind(_signals.Incoming.AddShield).ToSequence<AddShieldCommand>();
            CommandBinder.Bind(_signals.Incoming.StartRevive).ToSequence<StartReviveCommand>();
            CommandBinder.Bind(_signals.Incoming.TouchMove).ToSequence<StoreMoveInputCommand>();
            CommandBinder.Bind(_signals.Incoming.SetFriendlyFire).ToSequence<StoreFriendlyFireCommand>();
            CommandBinder.Bind(_internalSignals.LocalCameraRegistered)
                .ToSequence<LoadCameraSettingsCommand>()
                .ToSequence<PublishCameraSettingsCommand>();

            CommandBinder.Bind(_signals.Incoming.SetCameraSettings)
                .ToSequence<StoreCameraSettingsCommand>()
                .ToSequence<SaveCameraSettingsCommand>()
                .ToSequence<PublishCameraSettingsCommand>();

            CommandBinder.Bind(_signals.Incoming.ShakeCamera).ToSequence<SignalDispatchCommand>(_internalSignals.ShakeLocalCamera);
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
