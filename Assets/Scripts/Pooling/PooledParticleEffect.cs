using System.Collections;
using UnityEngine;

namespace FireLine.Scripts.Pooling
{
    public class PooledParticleEffect :
        MonoBehaviour,
        IPoolable
    {
        private IPoolService _poolService;

        private ParticleSystem _particleSystem;

        private string _poolKey;

        private Coroutine _returnCoroutine;

        private void Awake()
        {
            _particleSystem =
                GetComponent<ParticleSystem>();
        }

        public void Initialize(
            IPoolService poolService,
            string poolKey)
        {
            _poolService = poolService;
            _poolKey = poolKey;
        }

        public void OnSpawn()
        {
            if (_particleSystem == null)
            {
                Debug.LogError(
                    "[POOLED PARTICLE] " +
                    "ParticleSystem is NULL!"
                );

                return;
            }

            if (_poolService == null)
            {
                Debug.LogError(
                    "[POOLED PARTICLE] " +
                    "PoolService is NULL!"
                );

                return;
            }

            _particleSystem.Play();

            if (_returnCoroutine != null)
            {
                StopCoroutine(_returnCoroutine);
            }

            _returnCoroutine =
                StartCoroutine(ReturnToPool());
        }

        public void OnDespawn()
        {
            if (_returnCoroutine != null)
            {
                StopCoroutine(_returnCoroutine);

                _returnCoroutine = null;
            }

            if (_particleSystem != null)
            {
                _particleSystem.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear
                );
            }
        }

        private IEnumerator ReturnToPool()
        {
            yield return new WaitForSeconds(
                _particleSystem.main.duration
            );

            _poolService.Despawn(
                _poolKey,
                this
            );

            _returnCoroutine = null;
        }
    }
}