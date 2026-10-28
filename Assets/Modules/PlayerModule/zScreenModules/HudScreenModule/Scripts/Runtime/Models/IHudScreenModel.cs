namespace Modules.PlayerModule.HudScreenModule.Models
{
    /// <summary>The last values reported, for a HUD that opens after they arrived.</summary>
    internal interface IHudScreenModel
    {
        float Health { get; set; }
        float MaxHealth { get; set; }
        int Ammo { get; set; }
        int MagazineSize { get; set; }

        int Wave { get; set; }
        int ZombiesLeft { get; set; }

        /// <summary>Time.time the wave comes in at, so a HUD opened late counts down from the right place.</summary>
        float WaveStartTime { get; set; }

        float Shield { get; set; }
        int Money { get; set; }
        int Carried { get; set; }
        bool HasWallet { get; set; }
    }

    internal class HudScreenModel : IHudScreenModel
    {
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public int Ammo { get; set; }
        public int MagazineSize { get; set; }
        public int Wave { get; set; }
        public int ZombiesLeft { get; set; }
        public float WaveStartTime { get; set; }
        public float Shield { get; set; }
        public int Money { get; set; }
        public int Carried { get; set; }
        public bool HasWallet { get; set; }
    }
}
