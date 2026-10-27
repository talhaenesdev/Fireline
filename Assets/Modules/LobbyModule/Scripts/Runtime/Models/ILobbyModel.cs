using Modules.LobbyModule.Shared.Data.ValueObjects;
using Modules.MatchModule.Shared.Enums;
using Modules.NetworkSessionModule.Shared.Data.ValueObjects;

namespace Modules.LobbyModule.Models
{
    internal interface ILobbyModel
    {
        bool IsBusy { get; }
        string Status { get; }
        SessionStateVO Session { get; }
        GameMode Mode { get; set; }

        /// <summary>Takes the lobby for one operation. False when another one still holds it.</summary>
        bool TryClaim(string status);

        /// <summary>Gives the lobby back, leaving the status a finished operation reports.</summary>
        void Finish(string status);

        void SetSession(SessionStateVO session);

        LobbyStateVO ToStateVO();
    }
}
