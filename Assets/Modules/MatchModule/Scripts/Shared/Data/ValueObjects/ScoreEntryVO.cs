namespace Modules.MatchModule.Shared.Data.ValueObjects
{
    /// <summary>One row of the scoreboard.</summary>
    public readonly struct ScoreEntryVO
    {
        public readonly ulong ClientId;
        public readonly string PlayerName;
        public readonly int Kills;
        public readonly int Deaths;

        public ScoreEntryVO(ulong clientId, string playerName, int kills, int deaths)
        {
            ClientId = clientId;
            PlayerName = playerName;
            Kills = kills;
            Deaths = deaths;
        }

        public float Kda => Deaths == 0 ? Kills : (float) Kills / Deaths;
    }
}
