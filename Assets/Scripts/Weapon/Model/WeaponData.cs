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

        public BulletData BulletData => bulletData;
        public float FireRate => fireRate;
        public AudioClip[] FireClips => fireClips;
        public bool Automatic => automatic;
    }
}