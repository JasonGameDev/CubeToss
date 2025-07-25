using UnityEngine;
using CubeToss.Gameplay;

namespace CubeToss.Environment
{
    public class Planet : MonoBehaviour
    {
        [SerializeField] private ScoreModule scoreModule;
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<GrabbableObject>(out var _))
            {
                Destroy(gameObject);

                scoreModule.ScoreImpact(other.attachedRigidbody.linearVelocity, 1000);
            }
        }
    }
}