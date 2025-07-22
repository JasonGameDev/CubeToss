using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CubeToss.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrabbableObject : MonoBehaviour
    {
        public enum GrabState
        {
            Idle,
            Grabbing,
            Held,
            Returning,
            Released
        }

        [SerializeField] private GrabState state = GrabState.Idle;
        public GrabState State => state;

        [SerializeField] private float returnSpeed = 8f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float heldFollowSpeed = 15f;

        public UnityEvent GrabStarted;
        public UnityEvent GrabHeld;
        public UnityEvent GrabCanceled;
        public UnityEvent GrabReleased;

        private Vector3 _savedPosition;
        private Quaternion _savedRotation;

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        
        private float _grabSpeed;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _savedPosition = transform.position;
            _savedRotation = transform.rotation;
        }

        private void Update()
        {
            switch (state)
            {
                case GrabState.Grabbing:
                    MoveAndRotate(_targetPosition, _targetRotation, _grabSpeed, rotationSpeed);
                    break;
                case GrabState.Held:
                    MoveAndRotate(_targetPosition, _targetRotation, heldFollowSpeed, rotationSpeed);
                    break;
                case GrabState.Returning:
                    MoveAndRotate(_savedPosition, _savedRotation, returnSpeed, rotationSpeed);
                    if (Vector3.Distance(transform.position, _savedPosition) < 0.001f && Quaternion.Angle(transform.rotation, _savedRotation) < 0.5f)
                        state = GrabState.Idle;
                    break;
                case GrabState.Released:
                    if (_rigidbody.linearVelocity.sqrMagnitude < 0.001f && _rigidbody.angularVelocity.sqrMagnitude < 0.001f)
                        state = GrabState.Idle;
                    break;
                case GrabState.Idle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void StartGrab(Vector3 targetPosition, float speed)
        {
            if (state != GrabState.Idle)
                return;

            _savedPosition = transform.position;
            _savedRotation = transform.rotation;
            _rigidbody.isKinematic = true;
            _grabSpeed = speed;
            _targetRotation = Quaternion.identity;

            state = GrabState.Grabbing;
            GrabStarted?.Invoke();

            _targetPosition = targetPosition;
        }
        
        public void UpdateGrab(Vector3 targetPosition, float speed)
        {
            if (state != GrabState.Grabbing)
                return;

            _targetPosition = targetPosition;
            _grabSpeed = speed;
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
            if (state != GrabState.Grabbing) 
                return;

            state = GrabState.Returning;
            GrabCanceled?.Invoke();
        }

        public void ReleaseGrab(Vector3 velocity)
        {
            if (state != GrabState.Held)
                return;

            _rigidbody.isKinematic = false;
            _rigidbody.linearVelocity = velocity;
            
            state = GrabState.Released;
            GrabReleased?.Invoke();
        }
        
        private void MoveAndRotate(Vector3 targetPos, Quaternion targetRot, float posSpeed, float rotSpeed)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, posSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
        }
    }
}
