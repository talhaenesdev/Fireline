using FlowIoC.BaseModule.Controller;
using UnityEngine;

namespace Modules.MainModule.Controllers
{
    internal class QuitApplicationCommand : Command
    {
        public override void Execute()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
