using Unity.Netcode;
using UnityEngine;

public class NetworkGameTest : MonoBehaviour
{
    private void Start()
    {
        NetworkManager networkManager =
            NetworkManager.Singleton;

        if (networkManager == null)
        {
            Debug.LogError(
                "[GAME TEST] NetworkManager NULL!"
            );

            return;
        }

        Debug.Log(
            $"[GAME TEST] Game Scene Loaded | " +
            $"IsServer: {networkManager.IsServer} | " +
            $"IsHost: {networkManager.IsHost} | " +
            $"IsClient: {networkManager.IsClient} | " +
            $"ClientId: {networkManager.LocalClientId}"
        );
    }
}