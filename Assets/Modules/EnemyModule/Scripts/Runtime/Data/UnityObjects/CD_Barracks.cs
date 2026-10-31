using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>
    /// A barracks a player buys: the building itself, how often it trains a soldier and how many it
    /// keeps, and how those soldiers fight. Soldiers stand guard around their barracks, go for the
    /// zombies that come near it and shoot for the barracks' owner.
    /// </summary>
    [CreateAssetMenu(fileName = "CD_Barracks", menuName = "Fireline/Enemy/CD_Barracks")]
    public class CD_Barracks : ScriptableObject
    {
        [Header("Building")]
        [SerializeField] private int _health = 500;
        [SerializeField] private float _maxPlaceDistance = 8f;

        [Header("Training")]
        [Tooltip("The networked soldier prefab, drawn from the Ally pool.")]
        [SerializeField] private GameObject _allyPrefab;
        [SerializeField] private string _allyPoolKey = "Ally";
        [Tooltip("Seconds between two soldiers.")]
        [SerializeField] private float _trainInterval = 12f;
        [Tooltip("Soldiers one barracks keeps alive at once.")]
        [SerializeField] private int _maxAllies = 3;
        [Tooltip("How far around the barracks its soldiers stand guard.")]
        [SerializeField] private float _postRadius = 2.5f;

        [Header("Soldier")]
        [SerializeField] private int _allyHealth = 120;
        [SerializeField] private float _allyMoveSpeed = 3.2f;
        [SerializeField] private int _allyDamage = 12;
        [SerializeField] private float _allyRange = 9f;
        [SerializeField] private float _allyFireInterval = 0.6f;
        [Tooltip("Soldiers go after zombies this close to their post, and no further.")]
        [SerializeField] private float _guardRadius = 14f;
        [SerializeField] private Color _allyTint = new(0.35f, 0.6f, 1f, 1f);

        public int Health => _health;
        public float MaxPlaceDistance => _maxPlaceDistance;
        public GameObject AllyPrefab => _allyPrefab;
        public string AllyPoolKey => _allyPoolKey;
        public float TrainInterval => _trainInterval;
        public int MaxAllies => _maxAllies;
        public float PostRadius => _postRadius;
        public int AllyHealth => _allyHealth;
        public float AllyMoveSpeed => _allyMoveSpeed;
        public int AllyDamage => _allyDamage;
        public float AllyRange => _allyRange;
        public float AllyFireInterval => _allyFireInterval;
        public float GuardRadius => _guardRadius;
        public Color AllyTint => _allyTint;
    }
}
