using FireLine.Scripts.Weapon.Controller;
using FireLine.Scripts.Weapon.Model;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Weapon.Installer
{
    public class WeaponInstaller : MonoInstaller
    {
        [SerializeField]
        private WeaponData weaponData;

        public override void InstallBindings()
        {
            Debug.Log(
                $"[WEAPON INSTALLER][START] " +
                $"GameObject={gameObject.name} | " +
                $"Scene={gameObject.scene.name} | " +
                $"Container={Container.GetHashCode()}"
            );

            if (weaponData == null)
            {
                Debug.LogError(
                    $"[WEAPON INSTALLER][ERROR] " +
                    $"WeaponData is NULL | " +
                    $"GameObject={gameObject.name} | " +
                    $"Scene={gameObject.scene.name}"
                );

                return;
            }

            Debug.Log(
                $"[WEAPON INSTALLER][DATA] " +
                $"WeaponData={weaponData.name} | " +
                $"Instance={weaponData.GetInstanceID()}"
            );

            Container.Bind<WeaponData>()
                .FromInstance(weaponData)
                .AsSingle();

            Debug.Log(
                $"[WEAPON INSTALLER][BIND] " +
                $"WeaponData bound | " +
                $"Container={Container.GetHashCode()}"
            );

            Container.Bind<WeaponController>()
                .AsTransient();

            Debug.Log(
                $"[WEAPON INSTALLER][BIND] " +
                $"WeaponController bound | " +
                $"Container={Container.GetHashCode()}"
            );

            Debug.Log(
                $"[WEAPON INSTALLER][DONE] " +
                $"Scene={gameObject.scene.name} | " +
                $"Container={Container.GetHashCode()}"
            );
        }
    }
}
