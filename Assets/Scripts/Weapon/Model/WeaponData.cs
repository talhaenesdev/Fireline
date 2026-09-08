using UnityEngine;
using static UnityEngine.Android.AndroidGame;

namespace FireLine.Scripts.Weapon.Model
{
    [CreateAssetMenu(
        fileName = "WeaponData",
        menuName = "FireLine/Weapon/Weapon Data"
    )]
    public class WeaponData : ScriptableObject
    {
        [Header("Weapon")]
        [SerializeField]
        private BulletData bulletData;

        [SerializeField]
        private float fireRate = 0.25f;

        [SerializeField]
        private bool automatic = true;

        [Header("Audio")]
        [SerializeField]
        private AudioClip[] fireClips;

        [Header("Magazine")]
        [SerializeField]
        private int magazineSize = 7;

        [SerializeField]
        private float reloadDuration = 1.5f;


        public int MagazineSize => magazineSize;
        public float ReloadDuration => reloadDuration;
        public BulletData BulletData => bulletData;
        public float FireRate => fireRate;
        public AudioClip[] FireClips => fireClips;
        public bool Automatic => automatic;
    }
}