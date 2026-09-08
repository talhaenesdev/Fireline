using UnityEngine;

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

        [Header("Magazine")]
        [SerializeField]
        private int magazineSize = 7;

        [SerializeField]
        private float reloadDuration = 1.5f;

        [Header("Audio")]
        [SerializeField]
        private AudioClip[] fireClips;

        public BulletData BulletData =>
            bulletData;

        public float FireRate =>
            fireRate;

        public bool Automatic =>
            automatic;

        public int MagazineSize =>
            magazineSize;

        public float ReloadDuration =>
            reloadDuration;

        public AudioClip[] FireClips =>
            fireClips;
    }
}