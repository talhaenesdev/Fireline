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
            Debug.Log(
                "[UI INSTALLER] " +
                "InstallBindings called."
            );

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

            Container.BindInterfacesTo<UIAudioService>()
                .AsSingle()
                .NonLazy();

            Debug.Log(
                "[UI INSTALLER] " +
                "UIAudioService bound."
            );
        }
    }
}