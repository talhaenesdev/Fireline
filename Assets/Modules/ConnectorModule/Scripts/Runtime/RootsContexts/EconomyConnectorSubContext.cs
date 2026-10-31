using FlowIoC.BaseModule.Connectors;
using FlowIoC.BaseModule.Contexts;
using Modules.EconomyModule.ShopScreenModule.Signals;
using Modules.EconomyModule.Signals;
using Modules.EnemyModule.Signals;
using Modules.PlayerModule.HudScreenModule.Signals;
using Modules.PlayerModule.Signals;
using Modules.WeaponModule.Signals;

namespace Modules.ConnectorModule.RootsContexts
{
    /// <summary>
    /// The zombie match's economy: fallen zombies drop loot, the wallet shows on the HUD and in the
    /// shop, the shop zone opens the shop, and what is bought reaches the player and the weapon.
    /// </summary>
    public class EconomyConnectorSubContext : Context
    {
        private const string GROUP = nameof(EconomyConnectorSubContext);

        private EconomySignals _economySignals;
        private ShopScreenSignals _shopScreenSignals;
        private EnemySignals _enemySignals;
        private HudScreenSignals _hudScreenSignals;
        private PlayerSignals _playerSignals;
        private WeaponSignals _weaponSignals;

        public override void Setup()
        {
            base.Setup();

            _economySignals = InjectionBinderCrossContext.GetInstance<EconomySignals>();
            _shopScreenSignals = InjectionBinderCrossContext.GetInstance<ShopScreenSignals>();
            _enemySignals = InjectionBinderCrossContext.GetInstance<EnemySignals>();
            _hudScreenSignals = InjectionBinderCrossContext.GetInstance<HudScreenSignals>();
            _playerSignals = InjectionBinderCrossContext.GetInstance<PlayerSignals>();
            _weaponSignals = InjectionBinderCrossContext.GetInstance<WeaponSignals>();

            IncomingSignals();
            OutgoingSignals();
        }

        private void IncomingSignals()
        {
            _enemySignals.Outgoing.ZombieDied.Connect(_economySignals.Incoming.DropLoot, GROUP);
            _shopScreenSignals.Outgoing.BuyClicked.Connect(_economySignals.Incoming.Buy, GROUP);
            _enemySignals.Outgoing.WaveCleared.Connect(_economySignals.Incoming.WaveCleared, GROUP);
            _enemySignals.Outgoing.ZombieKilled.Connect(_economySignals.Incoming.CreditKill, GROUP);
            _enemySignals.Outgoing.ObjectiveRewarded.Connect(_economySignals.Incoming.PayEveryone, GROUP);
        }

        private void OutgoingSignals()
        {
            _economySignals.Outgoing.LocalWalletChanged.Connect(_hudScreenSignals.Incoming.ApplyWallet, GROUP);
            _economySignals.Outgoing.WaveRewardAnnounced.Connect(_hudScreenSignals.Incoming.ApplyWaveCleared, GROUP);
            _economySignals.Outgoing.LocalProgressChanged.Connect(_hudScreenSignals.Incoming.ApplyProgress, GROUP);
            // The shop shows only the money; the bag is the HUD's.
            _economySignals.Outgoing.LocalWalletChanged.Connect(_shopScreenSignals.Incoming.ApplyMoney, (money, carried) => money, GROUP);
            _economySignals.Outgoing.ShopEntered.Connect(_shopScreenSignals.Incoming.Open, GROUP);
            _economySignals.Outgoing.ShopLeft.Connect(_shopScreenSignals.Incoming.Close, GROUP);
            _economySignals.Outgoing.HealPurchased.Connect(_playerSignals.Incoming.Heal, GROUP);
            _economySignals.Outgoing.ShieldPurchased.Connect(_playerSignals.Incoming.AddShield, GROUP);
            _economySignals.Outgoing.FireRatePurchased.Connect(_weaponSignals.Incoming.ImproveFireRate, GROUP);
            _economySignals.Outgoing.ReloadPerkPurchased.Connect(_weaponSignals.Incoming.ImproveReload, GROUP);
            _economySignals.Outgoing.PerkPurchased.Connect(_playerSignals.Incoming.ApplyPerk, GROUP);
            _economySignals.Outgoing.TurretPurchased.Connect(_enemySignals.Incoming.GrantTurret, GROUP);
            _economySignals.Outgoing.BarricadePurchased.Connect(_enemySignals.Incoming.GrantBarricade, GROUP);
            _economySignals.Outgoing.WallPurchased.Connect(_enemySignals.Incoming.GrantWall, GROUP);
            _economySignals.Outgoing.BarracksPurchased.Connect(_enemySignals.Incoming.GrantBarracks, GROUP);
            _economySignals.Outgoing.TurretUpgradePurchased.Connect(_enemySignals.Incoming.UpgradeTurrets, GROUP);
            _economySignals.Outgoing.WeaponPurchased.Connect(_weaponSignals.Incoming.EquipWeapon, GROUP);
        }

        public override void DestroyContext()
        {
            SignalConnector.DisconnectGroup(GROUP);
            base.DestroyContext();
        }
    }
}
