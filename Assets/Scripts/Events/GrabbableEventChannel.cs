using CubeToss.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace CubeToss.Events
{
    /// <summary>
    /// Simple version of asset based event channel or system module for basic event handling. 
    /// </summary>
    [CreateAssetMenu(fileName = "GrabbableEventChannel", menuName = "Scriptable Objects/GrabbableEventChannel")]
    public class GrabbableEventChannel : ScriptableObject
    {
        public UnityEvent<GrabbableObject> GrabStarted;
        public UnityEvent<GrabbableObject> GrabHeld;
        public UnityEvent<GrabbableObject> GrabCanceled;
        public UnityEvent<GrabbableObject> GrabReleased;
        public UnityEvent<GrabbableObject> EnteredPlane;
    }
}
