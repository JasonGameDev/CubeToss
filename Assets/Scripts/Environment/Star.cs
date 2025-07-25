using UnityEngine;
using CubeToss.Gameplay;

namespace CubeToss.Environment
{
    public class Star : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<GrabbableObject>(out var grabbable))
            {
                Destroy(grabbable.gameObject);
            }
        }
    }
}