using Modules.LobbyModule.Shared.Data.ValueObjects;

namespace Modules.LobbyModule.LobbyScreenModule.Models
{
    /// <summary>The last state the lobby reported, kept for a panel that opens after it arrived.</summary>
    internal interface ILobbyScreenModel
    {
        bool HasState { get; }
        LobbyStateVO State { get; set; }
    }
}
