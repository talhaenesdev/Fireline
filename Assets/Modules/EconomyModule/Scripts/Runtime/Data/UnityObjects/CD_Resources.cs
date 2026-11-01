using UnityEngine;

namespace Modules.EconomyModule.Data.UnityObjects
{
    /// <summary>Materials: what structures are built from besides money, gathered by standing in a scrap pile.</summary>
    [CreateAssetMenu(fileName = "CD_Resources", menuName = "Fireline/Economy/CD_Resources")]
    public class CD_Resources : ScriptableObject
    {
        [Tooltip("Materials a player standing in a scrap pile gathers each second.")]
        [SerializeField] private float _gatherPerSecond = 3f;
        [Tooltip("Materials every player starts the match with.")]
        [SerializeField] private int _startMaterials = 20;
        [Tooltip("The most materials one player can hold.")]
        [SerializeField] private int _maxMaterials = 300;

        public float GatherPerSecond => _gatherPerSecond;
        public int StartMaterials => _startMaterials;
        public int MaxMaterials => _maxMaterials;
    }
}
