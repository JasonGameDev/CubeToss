using System;
using UnityEngine;
using UnityEngine.Events;
using CubeToss.Events;
using Random = UnityEngine.Random;

namespace CubeToss.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrabbableObject : MonoBehaviour
    {
        public enum GrabState { Idle, Grabbing, Held, Returning, Released }
        [SerializeField] private GrabState state = GrabState.Idle;
        public GrabState State => state;

        // System wide Event Channel for Grabbable Objects
        public GrabbableEventChannel EventChannel;
        
        // On Object Grabable Events, easy to config vfx, etc, via Inspector
        public UnityEvent GrabStarted, GrabHeld, GrabCanceled, GrabReleased;

        private Vector3 _savedPosition;
        private Quaternion _savedRotation;

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        
        private float _grabSpeed;
        private float _returnSpeed;
        private float _rotationSpeed;
        private float _followSpeed;
        
        private Rigidbody _rigidbody;
        private Collider _collider;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            _savedPosition = transform.position;
            _savedRotation = transform.rotation;
            
            _collider.isTrigger = true;
            _rigidbody.isKinematic = true;
        }

        private void Update()
        {
            switch (state)
            {
                case GrabState.Grabbing:
                    MoveAndRotate(_targetPosition, _targetRotation, _grabSpeed, 0.0f);
                    break;
                
                case GrabState.Returning:
                    MoveAndRotate(_savedPosition, _savedRotation, _returnSpeed, _rotationSpeed);
                    if (Vector3.Distance(transform.position, _savedPosition) < 0.001f && Quaternion.Angle(transform.rotation, _savedRotation) < 0.5f)
                        state = GrabState.Idle;
                    break;
                
                case GrabState.Held:
                    MoveAndRotate(_targetPosition, _targetRotation, _followSpeed, _rotationSpeed);
                    break;
                
                case GrabState.Released:
                    // Under physics simulation, so no need to update position or rotation here.
                    // Debug.Log("Velocity: " + _rigidbody.linearVelocity + ", Angular Velocity: " + _rigidbody.angularVelocity);
                    break;
            }
        }

        public void StartGrab(Vector3 targetPosition, float grabSpeed, float returnSpeed, float followSpeed, float rotationSpeed)
        {
            if (state != GrabState.Idle)
                return;

            _rigidbody.isKinematic = true;
            
            _savedPosition = transform.position;
            _savedRotation = transform.rotation;
            
            _grabSpeed = grabSpeed;
            _returnSpeed = returnSpeed;
            _followSpeed = followSpeed;
            _rotationSpeed = rotationSpeed;
            
            _targetPosition = targetPosition;
            _targetRotation = Quaternion.identity;

            state = GrabState.Grabbing;
            GrabStarted?.Invoke();
        }
        
        public void UpdateGrab(Vector3 targetPosition)
        {
            if (state != GrabState.Grabbing)
                return;

            _targetPosition = targetPosition;
        }

        public void CompleteGrab()
        {
            if (state != GrabState.Grabbing)
                return;

            state = GrabState.Held;
            GrabHeld.Invoke();
        }

        public void HoldGrab(Vector3 targetPosition)
        {
            if (state != GrabState.Held)
                return;

            _targetPosition = targetPosition;
        }

        public void CancelGrab()
        {
            if (state != GrabState.Grabbing && state != GrabState.Held)
                return;

            state = GrabState.Returning;
            GrabCanceled?.Invoke();
        }

        public void ReleaseGrab(Vector3 velocity, Vector3 angularVelocity)
        {
            if (state != GrabState.Held)
                return;

            _collider.isTrigger = false;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.AddForce(velocity, ForceMode.VelocityChange);
            _rigidbody.AddTorque(angularVelocity, ForceMode.VelocityChange);
            
            state = GrabState.Released;
            GrabReleased?.Invoke();
            EventChannel.Released.Invoke(this);
        }
        
        private void MoveAndRotate(Vector3 targetPos, Quaternion targetRot, float posSpeed, float rotSpeed)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, posSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 20.0f);

            if (_rigidbody != null)
            {
                Gizmos.color = Color.cyan;
                Vector3 velocity = Application.isPlaying ? _rigidbody.linearVelocity : Vector3.zero;
                float velocityScale = 1.0f;
                Vector3 endPoint = transform.position + velocity * velocityScale;
                Gizmos.DrawLine(transform.position, endPoint);
                Gizmos.DrawWireSphere(endPoint, 10.0f);
            }
        }
    }
}
