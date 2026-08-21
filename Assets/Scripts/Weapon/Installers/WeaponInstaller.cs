using UnityEngine;
using Zenject;
using FireLine.Scripts.Weapon.Controller;
using FireLine.Scripts.Weapon.Model;

namespace FireLine.Scripts.Weapon
{
    public class WeaponInstaller : MonoInstaller
    {
        [SerializeField]
        private WeaponData weaponData;

        public override void InstallBindings()
        {
            Debug.Log("WEAPON INSTALLER RUNNING");
            Container.Bind<WeaponData>()
                .FromInstance(weaponData)
                .AsSingle();

            Container.Bind<WeaponController>()
                .AsTransient();
        }
    }
}