using FireLine.Scripts.Player.Controller;
using FireLine.Scripts.Player.Model;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Player.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerInputController>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<PlayerAimController>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<PlayerWeaponController>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<PlayerMovementController>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<PlayerModel>()
                .AsSingle()
                .WithArguments(5f);
        }
    }
}