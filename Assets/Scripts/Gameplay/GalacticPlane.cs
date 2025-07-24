using CubeToss.Events;
using UnityEngine;

namespace CubeToss.Gameplay
{
    public class GalacticPlane : MonoBehaviour
    {
        [SerializeField] GrabbableEventChannel grabbableEventChannel;
        
        private void OnTriggerEnter(Collider other)
        {
            var grabbable = other.GetComponent<GrabbableObject>();
            if (grabbable && grabbable.State == GrabbableObject.GrabState.Released)
                grabbableEventChannel.EnteredPlane?.Invoke(grabbable);
        }
    }
}
