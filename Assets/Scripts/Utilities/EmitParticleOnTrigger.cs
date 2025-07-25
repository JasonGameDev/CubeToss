using UnityEngine;

namespace CubeToss.Utilities
{
    [RequireComponent(typeof(Collider))]
    public class EmitParticlesOnTrigger : MonoBehaviour
    {
        [SerializeField] private ParticlePool particlePool;
        
        private void OnTriggerEnter(Collider other)
        {
            particlePool.EmitParticle(other.transform.position);
        }
    }
}