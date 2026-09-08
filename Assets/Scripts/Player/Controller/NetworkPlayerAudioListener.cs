using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Player.Controller
{
    public class NetworkPlayerAudioListener : NetworkBehaviour
    {
        private AudioListener _audioListener;

        private void Awake()
        {
            _audioListener =
                GetComponentInChildren<AudioListener>(true);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (_audioListener == null)
            {
                Debug.LogError(
                    "[PLAYER AUDIO] AudioListener not found!"
                );

                return;
            }

            _audioListener.enabled = IsOwner;

            Debug.Log(
                $"[PLAYER AUDIO] " +
                $"Owner={IsOwner} | " +
                $"AudioListener={_audioListener.enabled}"
            );
        }
    }
}