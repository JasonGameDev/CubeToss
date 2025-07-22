using System;
using CubeToss.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CubeToss.Gameplay
{
    public class Grabber : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        
        [SerializeField] private float grabDistance = 0.3f;
        [SerializeField] private float holdDistance = 0.1f;
        
        [SerializeField] private float grabSpeed = 5f;
        
        [SerializeField] private float positionError = 0.01f;

        private CubeToss_InputActions _inputActions;

        private GrabbableObject _currentGrabbable;
        private Vector3 _lastWorldPosition;
        private Vector3 _velocity;

        private void Awake()
        {
            _inputActions = new CubeToss_InputActions();
            _inputActions.Enable();

            _inputActions.Player.Grab.started += OnGrabStarted;
            _inputActions.Player.Grab.canceled += OnGrabCanceled;
        }

        private void OnDestroy()
        {
            _inputActions.Player.Grab.started -= OnGrabStarted;
            _inputActions.Player.Grab.canceled -= OnGrabCanceled;
            _inputActions.Disable();
        }

        private void Update()
        {
            if (!_currentGrabbable)
                return;

            var pointerPos = _inputActions.Player.PointerPosition.ReadValue<Vector2>();
            var ray = mainCamera.ScreenPointToRay(pointerPos);
            var holdPoint = ray.GetPoint(holdDistance);
            var distance = Vector3.Distance(_currentGrabbable.transform.position, holdPoint);

            switch (_currentGrabbable.State)
            {
                case GrabbableObject.GrabState.Idle:
                    _currentGrabbable.StartGrab(holdPoint, grabSpeed);
                    break;
                case GrabbableObject.GrabState.Grabbing:
                    _currentGrabbable.UpdateGrab(holdPoint, grabSpeed);
                    
                    if (distance <= positionError)
                        _currentGrabbable.CompleteGrab();

                    break;
                case GrabbableObject.GrabState.Held:
                    _velocity = (holdPoint - _lastWorldPosition) / Time.deltaTime;
                    _lastWorldPosition = holdPoint;
                    _currentGrabbable.HoldGrab(holdPoint);
                    break;
                case GrabbableObject.GrabState.Returning:
                case GrabbableObject.GrabState.Released:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnGrabStarted(InputAction.CallbackContext ctx)
        {
            var pointer = _inputActions.Player.PointerPosition.ReadValue<Vector2>();
            var ray = mainCamera.ScreenPointToRay(pointer);

            if (Physics.Raycast(ray, out RaycastHit hit, grabDistance) && hit.collider.TryGetComponent(out GrabbableObject grabbable))
            {
                _currentGrabbable = grabbable;
                _lastWorldPosition = grabbable.transform.position;
            }
        }

        private void OnGrabCanceled(InputAction.CallbackContext ctx)
        {
            if (!_currentGrabbable) 
                return;

            switch (_currentGrabbable.State)
            {
                case GrabbableObject.GrabState.Grabbing:
                    _currentGrabbable.CancelGrab();
                    break;
                case GrabbableObject.GrabState.Held:
                    _currentGrabbable.ReleaseGrab(_velocity);
                    break;
                case GrabbableObject.GrabState.Idle:
                case GrabbableObject.GrabState.Returning:
                case GrabbableObject.GrabState.Released:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _currentGrabbable = null;
        }
    }
}
