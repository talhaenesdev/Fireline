using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>A mine a player buys: the first zombie that comes near sets it off, and the blast hurts every zombie around.</summary>
    [CreateAssetMenu(fileName = "CD_Mine", menuName = "Fireline/Enemy/CD_Mine")]
    public class CD_Mine : ScriptableObject
    {
        [SerializeField] private int _health = 40;
        [SerializeField] private float _maxPlaceDistance = 6f;
        [SerializeField] private float _triggerRadius = 1.2f;
        [SerializeField] private float _blastRadius = 4f;
        [SerializeField] private int _damage = 120;
        [SerializeField] private Color _blastColor = new(1f, 0.6f, 0.15f, 1f);

        public int Health => _health;
        public float MaxPlaceDistance => _maxPlaceDistance;
        public float TriggerRadius => _triggerRadius;
        public float BlastRadius => _blastRadius;
        public int Damage => _damage;
        public Color BlastColor => _blastColor;
    }
}
