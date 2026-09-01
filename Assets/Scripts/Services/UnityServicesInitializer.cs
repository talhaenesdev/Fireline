using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace FireLine.Scripts.Services
{
    public class UnityServicesInitializer :
        MonoBehaviour
    {
        public static UnityServicesInitializer Instance
        {
            get;
            private set;
        }

        public bool IsReady
        {
            get;
            private set;
        }

        private void Awake()
        {
            if (Instance != null &&
                Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            await Initialize();
        }

        public async Task<bool> Initialize()
        {
            if (IsReady)
                return true;

            try
            {
                if (UnityServices.State ==
                    ServicesInitializationState.Initialized)
                {
                    Debug.Log(
                        "[UGS] Services already initialized."
                    );
                }
                else
                {
                    await UnityServices.InitializeAsync();

                    Debug.Log(
                        "[UGS] Services initialized."
                    );
                }

                if (!AuthenticationService.Instance
                        .IsSignedIn)
                {
                    await AuthenticationService.Instance
                        .SignInAnonymouslyAsync();

                    Debug.Log(
                        $"[UGS] Anonymous sign-in successful | " +
                        $"PlayerId: " +
                        $"{AuthenticationService.Instance.PlayerId}"
                    );
                }
                else
                {
                    Debug.Log(
                        $"[UGS] Already signed in | " +
                        $"PlayerId: " +
                        $"{AuthenticationService.Instance.PlayerId}"
                    );
                }

                IsReady = true;

                Debug.Log(
                    "[UGS] Initialization completed."
                );

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[UGS] Initialization failed | " +
                    $"{exception}"
                );

                IsReady = false;

                return false;
            }
        }
    }
}