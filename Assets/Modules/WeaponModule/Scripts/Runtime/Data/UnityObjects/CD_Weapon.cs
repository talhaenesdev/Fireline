using UnityEngine;

namespace Modules.WeaponModule.Data.UnityObjects
{
    /// <summary>One weapon's handling and the rounds it fires.</summary>
    [CreateAssetMenu(fileName = "CD_Weapon", menuName = "Fireline/Weapon/CD_Weapon")]
    public class CD_Weapon : ScriptableObject
    {
        [Tooltip("What the HUD and the shop call it.")]
        [SerializeField] private string _displayName = "Tabanca";

        [Header("Handling")]
        [SerializeField] private float _fireRate = 0.25f;
        [SerializeField] private bool _automatic = true;
        [SerializeField] private int _magazineSize = 7;
        [SerializeField] private float _reloadDuration = 1.5f;

        [Header("Rounds")]
        [SerializeField] private int _damage = 10;
        [SerializeField] private float _bulletSpeed = 20f;
        [SerializeField] private float _bulletLifetime = 3f;
        [Tooltip("Rounds each shot fires; a shotgun fires several.")]
        [SerializeField] private int _pellets = 1;
        [Tooltip("How far, in degrees either side, a round may stray from the aim.")]
        [SerializeField] private float _spreadAngle;
        [SerializeField] private GameObject _networkBulletPrefab;
        [SerializeField] private string _predictedBulletPoolKey = "PredictedBullet";
        [Tooltip("The pool the server's rounds come from, through Netcode.")]
        [SerializeField] private string _networkBulletPoolKey = "NetworkBullet";
        [SerializeField] private string _impactPoolKey = "WallImpact";

        public string DisplayName => _displayName;
        public int Pellets => Mathf.Max(1, _pellets);
        public float SpreadAngle => _spreadAngle;
        public float FireRate => _fireRate;
        public bool Automatic => _automatic;
        public int MagazineSize => _magazineSize;
        public float ReloadDuration => _reloadDuration;
        public int Damage => _damage;
        public float BulletSpeed => _bulletSpeed;
        public float BulletLifetime => _bulletLifetime;
        public GameObject NetworkBulletPrefab => _networkBulletPrefab;
        public string PredictedBulletPoolKey => _predictedBulletPoolKey;
        public string NetworkBulletPoolKey => _networkBulletPoolKey;
        public string ImpactPoolKey => _impactPoolKey;
    }
}
