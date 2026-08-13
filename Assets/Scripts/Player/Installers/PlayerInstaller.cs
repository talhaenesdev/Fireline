using FireLine.Scripts.Core.Services.Entities;
using FireLine.Scripts.Player.Controller;
using FireLine.Scripts.Player.Model;
using FireLine.Scripts.Player.View;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Player.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerView>()
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

            Container.Bind<PlayerController>()
                .AsTransient();

            Container.Bind<PlayerInputController>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}