using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using Modules.MatchModule.Shared.Constants;
using Modules.NetworkSessionModule.Services;
using UnityEngine.SceneManagement;

namespace Modules.MatchModule.Controllers
{
    /// <summary>Leaving drops the session and stops Netcode, whichever of the two fails.</summary>
    internal class LeaveNetworkCommand : Command
    {
        [Inject] private INetworkSessionService _session { get; set; }

        public override async void Execute()
        {
            Retain();

            try
            {
                await _session.LeaveSessionAsync();
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"LeaveNetworkCommand - leaving the session threw: {exception}");
            }

            _session.Shutdown();
            Release();
        }
    }

    internal class LoadMainSceneCommand : Command
    {
        public override void Execute() => SceneManager.LoadScene(MatchConstants.MAIN_SCENE);
    }
}
