using System.Collections;
using FlowIoC.PoolModule.Entities;
using UnityEngine;

namespace Modules.WeaponModule.Entities
{
    /// <summary>A particle burst that plays once where it is placed and goes back to its pool.</summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledParticleEffect : PoolableItem
    {
        private ParticleSystem _particleSystem;

        public override void OnInitialized() => _particleSystem = GetComponent<ParticleSystem>();

        public void PlayAt(Vector3 position)
        {
            transform.SetPositionAndRotation(position, Quaternion.identity);
            _particleSystem.Play();
            StartCoroutine(ReturnWhenDone());
        }

        public override void OnReturnToPool()
        {
            StopAllCoroutines();

            if (_particleSystem != null)
                _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private IEnumerator ReturnWhenDone()
        {
            yield return new WaitForSeconds(_particleSystem.main.duration);
            Dismiss();
        }
    }
}
