using UnityEngine;

namespace Modules.WeaponModule.Data.UnityObjects
{
    /// <summary>One weapon's handling and the rounds it fires.</summary>
    [CreateAssetMenu(fileName = "CD_Weapon", menuName = "Fireline/Weapon/CD_Weapon")]
    public class CD_Weapon : ScriptableObject
    {
        [Header("Handling")]
        [SerializeField] private float _fireRate = 0.25f;
        [SerializeField] private bool _automatic = true;
        [SerializeField] private int _magazineSize = 7;
        [SerializeField] private float _reloadDuration = 1.5f;

        [Header("Rounds")]
        [SerializeField] private int _damage = 10;
        [SerializeField] private float _bulletSpeed = 20f;
        [SerializeField] private float _bulletLifetime = 3f;
        [SerializeField] private GameObject _networkBulletPrefab;
        [SerializeField] private string _predictedBulletPoolKey = "PredictedBullet";
        [SerializeField] private string _impactPoolKey = "WallImpact";

        public float FireRate => _fireRate;
        public bool Automatic => _automatic;
        public int MagazineSize => _magazineSize;
        public float ReloadDuration => _reloadDuration;
        public int Damage => _damage;
        public float BulletSpeed => _bulletSpeed;
        public float BulletLifetime => _bulletLifetime;
        public GameObject NetworkBulletPrefab => _networkBulletPrefab;
        public string PredictedBulletPoolKey => _predictedBulletPoolKey;
        public string ImpactPoolKey => _impactPoolKey;
    }
}
