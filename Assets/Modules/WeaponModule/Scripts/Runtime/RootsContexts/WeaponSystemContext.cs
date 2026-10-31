using FlowIoC.BaseModule.Contexts;
using FlowIoC.BaseModule.Controller.Commands;
using Modules.AudioModule.Shared;
using Modules.WeaponModule.Controllers;
using Modules.WeaponModule.Models;
using Modules.WeaponModule.Signals;
using Modules.WeaponModule.ViewsMediators;

namespace Modules.WeaponModule.RootsContexts
{
    public class WeaponSystemContext : Context
    {
        private WeaponSignals _signals;
        private WeaponInternalSignals _internalSignals;
        private IWeaponModel _model;

        public override void SignalBindings()
        {
            base.SignalBindings();
            _internalSignals = InjectionBinder.Bind<WeaponInternalSignals>();
            _signals = InjectionBinderCrossContext.Bind<WeaponSignals>();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();
            _model = InjectionBinder.Bind<IWeaponModel, WeaponModel>();
        }

        public override void MediationBindings()
        {
            base.MediationBindings();
            MediationBinder.Bind<WeaponView>().To<WeaponMediator>();
            MediationBinder.Bind<NetworkBulletView>().To<NetworkBulletMediator>();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();

            CommandBinder.Bind(_internalSignals.WeaponSpawned)
                .ToSequence<PoolBulletsCommand>()
                .ToSequence<TrackServerWeaponCommand>()
                .ToSequence<RegisterLocalWeaponCommand>()
                .ToSequence<StartFrameTickCommand>()
                .ToSequence<PublishLocalWeaponCommand>()
                .ToSequence<PublishAmmoCommand>();

            CommandBinder.Bind(_internalSignals.WeaponDespawned)
                .ToSequence<UntrackServerWeaponCommand>()
                .ToSequence<UnregisterLocalWeaponCommand>();

            CommandBinder.Bind(_internalSignals.WeaponChanged)
                .ToSequence<ApplyLocalWeaponChangeCommand>()
                .ToSequence<PublishLocalWeaponCommand>()
                .ToSequence<PublishAmmoCommand>();

            CommandBinder.Bind(_signals.Incoming.EquipWeapon).ToSequence<EquipWeaponCommand>();

            CommandBinder.Bind(_signals.Incoming.FireInput).ToSequence<StoreTriggerCommand>();
            CommandBinder.Bind(_signals.Incoming.AimChanged).ToSequence<StoreAimCommand>();
            CommandBinder.Bind(_signals.Incoming.ImproveFireRate).ToSequence<ImproveFireRateCommand>();
            CommandBinder.Bind(_signals.Incoming.ImproveReload).ToSequence<ImproveReloadCommand>();
            CommandBinder.Bind(_signals.Incoming.SetArmed).ToSequence<StoreArmedCommand>();
            CommandBinder.Bind(_signals.Incoming.ReloadInput).ToSequence<SignalDispatchCommand>(_internalSignals.Reload);

            // One frame of the local weapon: reload when empty, then fire if this frame calls for it.
            CommandBinder.Bind(_internalSignals.Tick)
                .ToSequence<AutoReloadCommand>()
                .ToSequence<FireWeaponCommand>()
                .ToSequence<SpawnPredictedBulletCommand>()
                .ToSequence<RequestServerFireCommand>()
                .ToSequence<PlayLocalFireSoundCommand>()
                .ToSequence<SignalDispatchCommand>(_internalSignals.ShowLocalFire)
                .ToSequence<SignalDispatchCommand>(_signals.Outgoing.LocalFired)
                .ToSequence<PublishAmmoCommand>();

            CommandBinder.Bind(_internalSignals.Reload)
                .ToSequence<StartReloadCommand>()
                .ToSequence<PlayReloadSoundCommand>()
                .ToSequence<WaitReloadCommand>()
                .ToSequence<CompleteReloadCommand>()
                .ToSequence<PublishAmmoCommand>();

            CommandBinder.Bind(_internalSignals.ServerFireRequested)
                .ToSequence<SpawnServerBulletCommand>()
                .ToSequence<BroadcastFireCommand>();

            CommandBinder.Bind(_internalSignals.RemoteFired).ToSequence<PlaySoundAtPayloadCommand>(AudioKey.Weapon.Fire);

            CommandBinder.Bind(_internalSignals.BulletHit).ToSequence<ResolveBulletHitCommand>();

            CommandBinder.Bind(_internalSignals.ImpactShown)
                .ToSequence<PlayImpactEffectCommand>()
                .ToSequence<PlaySoundAtPayloadCommand>(AudioKey.Weapon.WallImpact);
        }

        public override void DestroyContext()
        {
            // The update loop outlives the scene; the flag lets its callback take itself off.
            if (_model != null)
            {
                _model.IsTicking = false;

                // The handler hands out this scene's pool; the next scene registers its own.
                if (Unity.Netcode.NetworkManager.Singleton != null)
                {
                    foreach (UnityEngine.GameObject prefab in _model.PooledBulletPrefabs)
                        Unity.Netcode.NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
                }

                _model.PooledBulletPrefabs.Clear();
            }

            base.DestroyContext();
        }
    }
}
