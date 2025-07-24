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
        public UnityEvent<GrabbableObject> Released;
        public UnityEvent<GrabbableObject> EnteredPlane;
    }
}
