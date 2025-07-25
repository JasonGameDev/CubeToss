using CubeToss.Events;
using UnityEngine;

namespace CubeToss.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class GalacticPlane : MonoBehaviour
    {
        [SerializeField] GrabbableEventChannel grabbableEventChannel;

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<GrabbableObject>(out var grabbable) && grabbable.State == GrabbableObject.GrabState.Released)
                grabbableEventChannel.EnteredPlane?.Invoke(grabbable);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_collider.bounds.Contains(other.transform.position))
                Destroy(other.gameObject);
        }
    }
}