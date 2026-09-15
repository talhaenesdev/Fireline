using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FireLine.Scripts.Core.Scene.Model
{
    [CreateAssetMenu(
        fileName = "SceneReference",
        menuName = "FireLine/Scene/Scene Reference"
    )]
    public class SceneReference : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField]
        private SceneAsset sceneAsset;
#endif

        [SerializeField]
        private string sceneName;

        public string SceneName =>
            sceneName;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(sceneName);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (sceneAsset == null)
            {
                sceneName = string.Empty;
                return;
            }

            string path =
                AssetDatabase.GetAssetPath(sceneAsset);

            sceneName =
                System.IO.Path.GetFileNameWithoutExtension(
                    path
                );
        }
#endif
    }
}