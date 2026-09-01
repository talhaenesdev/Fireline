using FireLine.Scripts.Core.Signals;
using FireLine.Scripts.Network.Signals;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Installers
{
    public class SignalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("[SIGNAL INSTALLER] Installing SignalBus...");

            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<EntityDestroyedSignal>();
            Container.DeclareSignal<NetworkPlayerDeathSignal>();
            Container.DeclareSignal<NetworkShootSignal>();

            Debug.Log("[SIGNAL INSTALLER] SignalBus + Signals installed.");
        }
    }
}