using UnityEngine;
using Zenject;
using FireLine.Scripts.Player.Model;
using FireLine.Scripts.Player.View;
using FireLine.Scripts.Player.Controller;
using FireLine.Scripts.Weapon.Model;
using FireLine.Scripts.Weapon.Controller;

namespace FireLine.Scripts.Player.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField]
        private WeaponData weaponData;

        public override void InstallBindings()
        {
            Container.Bind<PlayerModel>()
                .AsSingle()
                .WithArguments(5f);

            Container.Bind<PlayerView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<PlayerController>()
                .AsSingle();

            Container.BindInterfacesTo<PlayerInputController>()
                .AsSingle();

            Container.BindInterfacesTo<PlayerAimController>()
                .AsSingle();

            Container.Bind<Camera>()
                .FromInstance(Camera.main)
                .AsSingle();

            Container.Bind<WeaponData>()
                .FromInstance(weaponData)
                .AsSingle();

            Container.Bind<WeaponController>()
                .AsSingle();
        }
    }
}