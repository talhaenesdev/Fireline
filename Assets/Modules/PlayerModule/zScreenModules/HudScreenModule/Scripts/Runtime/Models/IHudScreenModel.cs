namespace Modules.PlayerModule.HudScreenModule.Models
{
    /// <summary>The last values reported, for a HUD that opens after they arrived.</summary>
    internal interface IHudScreenModel
    {
        float Health { get; set; }
        float MaxHealth { get; set; }
        int Ammo { get; set; }
        int MagazineSize { get; set; }
    }

    internal class HudScreenModel : IHudScreenModel
    {
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public int Ammo { get; set; }
        public int MagazineSize { get; set; }
    }
}
