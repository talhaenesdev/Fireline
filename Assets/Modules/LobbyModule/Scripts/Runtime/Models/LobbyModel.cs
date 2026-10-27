using Modules.LobbyModule.Shared.Constants;
using Modules.LobbyModule.Shared.Data.ValueObjects;
using Modules.MatchModule.Shared.Enums;
using Modules.NetworkSessionModule.Shared.Data.ValueObjects;

namespace Modules.LobbyModule.Models
{
    internal class LobbyModel : ILobbyModel
    {
        public bool IsBusy { get; private set; }
        public string Status { get; private set; } = LobbyConstants.STATUS_IDLE;
        public SessionStateVO Session { get; private set; } = SessionStateVO.None;
        public GameMode Mode { get; set; } = GameMode.Deathmatch;

        public bool TryClaim(string status)
        {
            if (IsBusy)
                return false;

            IsBusy = true;

            if (!string.IsNullOrEmpty(status))
                Status = status;

            return true;
        }

        public void Finish(string status)
        {
            IsBusy = false;

            if (!string.IsNullOrEmpty(status))
                Status = status;
        }

        public void SetSession(SessionStateVO session) => Session = session;

        public LobbyStateVO ToStateVO() => new(Session.HasSession, Session.IsHost, Session.JoinCode,
            Session.PlayerCount, Session.MaxPlayers, Session.IsLocalReady, Session.AreAllReady, IsBusy, Status, Mode);
    }
}
