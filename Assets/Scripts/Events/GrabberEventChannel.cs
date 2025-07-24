using UnityEngine;
using UnityEngine.Events;

namespace CubeToss.Events
{
    /// <summary>
    /// Simple version of asset based event channel or system module for basic event handling. 
    /// </summary>
    [CreateAssetMenu(fileName = "GrabberEventChannel", menuName = "Scriptable Objects/GrabberEventChannel")]
    public class GrabberEventChannel : ScriptableObject
    {
        public UnityEvent<float> UpdateFlickPower;
    }
}