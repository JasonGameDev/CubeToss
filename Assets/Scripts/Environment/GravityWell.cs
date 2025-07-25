using System.Collections.Generic;
using UnityEngine;
using CubeToss.Gameplay;

namespace cubeToss.Environment
{
    [RequireComponent(typeof(SphereCollider))]
    public class GravityWell : MonoBehaviour
    {
        [SerializeField] private float gravityStrength = 50f;
        [SerializeField] private float spiralStrength = 30f;

        private SphereCollider _collider;

        private readonly List<Rigidbody> _bodies = new();

        private void OnTriggerEnter(Collider other) 
        {
            if (other.TryGetComponent<GrabbableObject>(out var grabbable) && grabbable.State == GrabbableObject.GrabState.Released)
            {
                _bodies.Add(other.attachedRigidbody);
                other.attachedRigidbody.useGravity = false;
            }
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.TryGetComponent<GrabbableObject>(out var _))
            {
                _bodies.Remove(other.attachedRigidbody);
                other.attachedRigidbody.useGravity = true;
            }
        }

        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true;
        }

        private void FixedUpdate() 
        {
            var range = _collider.bounds.extents.x;
            
            for (int i = _bodies.Count - 1; i >= 0; i--)
            {
                var rb = _bodies[i];
                if (!rb)
                {
                    _bodies.RemoveAt(i);
                    continue;
                }
                
                var toCenter = transform.position - rb.position;
                var dist = toCenter.magnitude;
                var direction = toCenter.normalized;
                var strength = gravityStrength * (1f - Mathf.Clamp01(dist / range));
                var spiral = Vector3.Cross(direction, Vector3.up).normalized * spiralStrength;
                rb.AddForce((direction * strength + spiral) * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
        }
    }
}