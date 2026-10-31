using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>The defend objective: every few waves a point appears that the players must hold while the wave is on.</summary>
    [CreateAssetMenu(fileName = "CD_Objective", menuName = "Fireline/Enemy/CD_Objective")]
    public class CD_Objective : ScriptableObject
    {
        [SerializeField] private string _title = "Noktayı savun";
        [Tooltip("An objective comes with every wave whose number divides by this.")]
        [SerializeField] private int _everyWaves = 3;
        [Tooltip("Seconds a player has to stand in the point, in all, before the wave is cleared.")]
        [SerializeField] private float _holdSeconds = 20f;
        [SerializeField] private float _radius = 4f;
        [Tooltip("Money every player gets when it is held.")]
        [SerializeField] private int _reward = 120;

        public string Title => _title;
        public int EveryWaves => _everyWaves;
        public float HoldSeconds => _holdSeconds;
        public float Radius => _radius;
        public int Reward => _reward;
    }
}
