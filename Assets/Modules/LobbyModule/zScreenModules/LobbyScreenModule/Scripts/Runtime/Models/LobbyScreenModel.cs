using Modules.LobbyModule.Shared.Data.ValueObjects;

namespace Modules.LobbyModule.LobbyScreenModule.Models
{
    internal class LobbyScreenModel : ILobbyScreenModel
    {
        private LobbyStateVO _state;

        public bool HasState { get; private set; }

        public LobbyStateVO State
        {
            get => _state;
            set
            {
                _state = value;
                HasState = true;
            }
        }
    }
}
