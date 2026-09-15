using FireLine.Scripts.UI.Model;

namespace FireLine.Scripts.UI.Service
{
    public interface IUIAudioService
    {
        void PlayHover(UIButtonType type);

        void PlayClick(UIButtonType type);
    }
}