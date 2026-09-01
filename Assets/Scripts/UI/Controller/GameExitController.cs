using FireLine.Scripts.Network.Service;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace FireLine.Scripts.UI.Controller
{
    public class GameExitController : MonoBehaviour
    {
        private NetworkConnectionService _networkConnectionService;

        [Inject]
        public void Construct(
            NetworkConnectionService networkConnectionService)
        {
            _networkConnectionService =
                networkConnectionService;

            Debug.Log(
                "[GAME EXIT] NetworkConnectionService injected."
            );
        }

        public void ExitToMainMenu()
        {
            Debug.Log(
                "[GAME EXIT] Exiting game..."
            );

            if (_networkConnectionService != null)
            {
                _networkConnectionService.Shutdown();
            }
            else
            {
                Debug.LogError(
                    "[GAME EXIT] " +
                    "NetworkConnectionService is NULL!"
                );
            }

            SceneManager.LoadScene("MainMenu");
        }
    }
}