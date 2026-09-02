using FireLine.Scripts.Player.Controller;
using FireLine.Scripts.Player.Model;
using FireLine.Scripts.Player.Service;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Player.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerInputController>()
                .FromComponentOnRoot()
                .AsSingle();

            Container.Bind<PlayerMovementController>()
                .FromComponentOnRoot()
                .AsSingle();

            Container.Bind<PlayerAimController>()
                .FromComponentOnRoot()
                .AsSingle();

            Container.Bind<PlayerGameplayController>()
                .FromComponentOnRoot()
                .AsSingle();

            Container.Bind<PlayerModel>()
                .AsSingle()
                .WithArguments(5f);
        }
    }
}