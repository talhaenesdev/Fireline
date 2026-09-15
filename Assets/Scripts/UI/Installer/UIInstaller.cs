using FireLine.Scripts.UI.Model;
using FireLine.Scripts.UI.Service;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.UI.Installer
{
    public class UIInstaller : MonoInstaller
    {
        [Header("Audio")]
        [SerializeField]
        private UIButtonAudioData buttonAudioData;

        public override void InstallBindings()
        {
            BindAudio();
        }

        private void BindAudio()
        {
            if (buttonAudioData == null)
            {
                Debug.LogError(
                    "[UI INSTALLER] " +
                    "UIButtonAudioData is NULL!"
                );

                return;
            }

            Container.BindInstance(
                    buttonAudioData)
                .AsSingle();

            Container.Bind<IUIAudioService>()
                .To<UIAudioService>()
                .AsSingle()
                .NonLazy();

            Debug.Log(
                "[UI INSTALLER] " +
                "UI Audio Service bound."
            );
        }
    }
}