#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.IO;
using FlowIoC.ConsoleModule;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Modules.MainModule.Entities
{
    /// <summary>
    /// F12 saves a screenshot of the game, in the Editor and a Development Build only, for the store
    /// page. It lives for the whole run, across scenes, and decides nothing about the game.
    /// In the Editor the files land in Screenshots/ beside Assets; in a build, beside the save data.
    /// Set the Game view to 1920x1080 first; the shot is taken at the Game view's own size.
    /// </summary>
    public class ScreenshotTaker : MonoBehaviour
    {
        private const string FOLDER = "Screenshots";

        // The engine's entry point: the one place the taker is made, before the first scene loads.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            var go = new GameObject(nameof(ScreenshotTaker));
            DontDestroyOnLoad(go);
            go.AddComponent<ScreenshotTaker>();
        }

        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.f12Key.wasPressedThisFrame)
                return;

            string folder = Application.isEditor
                ? Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, FOLDER)
                : Path.Combine(Application.persistentDataPath, FOLDER);
            Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, $"Fireline_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png");
            ScreenCapture.CaptureScreenshot(path);
            FlowLogger.Log($"Screenshot - saved to {path}");
        }
    }
}
#endif
