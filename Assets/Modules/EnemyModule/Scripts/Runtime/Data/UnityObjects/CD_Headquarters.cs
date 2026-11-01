using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>The headquarters a base map starts with at its centre; when zombies bring it down the match is lost.</summary>
    [CreateAssetMenu(fileName = "CD_Headquarters", menuName = "Fireline/Enemy/CD_Headquarters")]
    public class CD_Headquarters : ScriptableObject
    {
        [SerializeField] private int _health = 3000;

        public int Health => _health;
    }
}
