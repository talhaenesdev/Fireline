using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>
    /// Day is the pause between waves, for building and gathering; night is the wave. Every so many
    /// nights a blood moon brings more zombies. The light is each client's; the timing is the server's.
    /// </summary>
    [CreateAssetMenu(fileName = "CD_DayNight", menuName = "Fireline/Enemy/CD_DayNight")]
    public class CD_DayNight : ScriptableObject
    {
        [Tooltip("Seconds of day between two waves; it replaces the wave table's pause.")]
        [SerializeField] private float _dayDuration = 35f;
        [Tooltip("Seconds the light takes to turn.")]
        [SerializeField] private float _transition = 3f;

        [Header("Light")]
        [SerializeField] private Color _dayColor = new(1f, 0.96f, 0.88f, 1f);
        [SerializeField] private float _dayIntensity = 1.1f;
        [SerializeField] private Color _nightColor = new(0.35f, 0.45f, 0.8f, 1f);
        [SerializeField] private float _nightIntensity = 0.35f;
        [SerializeField] private Color _bloodMoonColor = new(0.9f, 0.25f, 0.2f, 1f);
        [SerializeField] private float _bloodMoonIntensity = 0.5f;

        [Header("Blood moon")]
        [Tooltip("Every this many waves the night is a blood moon; zero for never.")]
        [SerializeField] private int _bloodMoonEvery = 4;
        [Tooltip("A blood moon brings this many times the wave's zombies.")]
        [SerializeField] private float _bloodMoonZombies = 1.6f;

        public float DayDuration => _dayDuration;
        public float Transition => _transition;
        public Color DayColor => _dayColor;
        public float DayIntensity => _dayIntensity;
        public Color NightColor => _nightColor;
        public float NightIntensity => _nightIntensity;
        public Color BloodMoonColor => _bloodMoonColor;
        public float BloodMoonIntensity => _bloodMoonIntensity;
        public float BloodMoonZombies => _bloodMoonZombies;

        public bool IsBloodMoon(int wave) => _bloodMoonEvery > 0 && wave > 0 && wave % _bloodMoonEvery == 0;
    }
}
