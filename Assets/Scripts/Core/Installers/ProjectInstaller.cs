using FireLine.Scripts.Player.Model;
using FireLine.Scripts.Pooling;
using FireLine.Scripts.Weapon.Model;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Core.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerModel>()
                .AsSingle()
                .WithArguments(5f);

        }
    }
}