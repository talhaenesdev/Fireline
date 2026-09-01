using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class NetworkLauncher : MonoBehaviour
    {
        private void OnGUI()
        {
            if (NetworkManager.Singleton == null)
                return;

            if (NetworkManager.Singleton.IsClient ||
                NetworkManager.Singleton.IsServer)
                return;

            GUILayout.BeginArea(new Rect(20, 20, 200, 150));

            if (GUILayout.Button("Start Host"))
            {
                NetworkManager.Singleton.StartHost();
            }

            if (GUILayout.Button("Start Client"))
            {
                NetworkManager.Singleton.StartClient();
            }

            GUILayout.EndArea();
        }
    }
}