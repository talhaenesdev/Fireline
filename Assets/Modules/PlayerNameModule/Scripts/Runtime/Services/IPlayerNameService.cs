namespace Modules.PlayerNameModule.Services
{
    /// <summary>The local player's display name, kept between runs.</summary>
    public partial interface IPlayerNameService
    {
        /// <summary>The stored name, or the default when none was ever stored.</summary>
        string Get();

        /// <summary>Cleans the name, stores it and answers what was stored.</summary>
        string Set(string playerName);

        /// <summary>The name as it would be stored: trimmed, never empty, at most 32 characters.</summary>
        string Clean(string playerName);

        public static partial class Commands
        {
        }
    }
}
