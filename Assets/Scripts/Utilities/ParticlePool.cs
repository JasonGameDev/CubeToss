using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeToss.Utilities
{
    public class ParticlePool : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particlePrefab;
        [SerializeField] private int poolSize = 10;

        private readonly Queue<ParticleSystem> _pool = new();

        private void Awake()
        {
            for (int i = 0; i < poolSize; i++)
            {
                var ps = Instantiate(particlePrefab, transform);
                ps.gameObject.SetActive(false);
                _pool.Enqueue(ps);
            }
        }

        public void EmitParticle(Vector3 worldPosition)
        {
            if (_pool.Count == 0)
            {
                Debug.LogWarning("Particle pool exhausted, consider increasing pool size.");
                return;
            }

            var ps = _pool.Dequeue();
            ps.transform.position = worldPosition;
            ps.gameObject.SetActive(true);
            ps.Play();

            float delay = ps.main.duration + ps.main.startLifetime.constantMax;
            StartCoroutine(ReturnToPool(ps, delay));
        }

        private IEnumerator ReturnToPool(ParticleSystem ps, float delay)
        {
            yield return new WaitForSeconds(delay);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.gameObject.SetActive(false);
            _pool.Enqueue(ps);
        }
    }
}