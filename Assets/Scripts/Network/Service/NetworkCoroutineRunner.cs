using System.Collections;
using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class NetworkCoroutineRunner : MonoBehaviour
    {
        public Coroutine Run(IEnumerator routine)
        {
            if (routine == null)
            {
                Debug.LogError(
                    "[NETWORK COROUTINE] Routine is NULL!"
                );

                return null;
            }

            return StartCoroutine(routine);
        }

        public void Stop(Coroutine coroutine)
        {
            if (coroutine == null)
                return;

            StopCoroutine(coroutine);
        }
    }
}