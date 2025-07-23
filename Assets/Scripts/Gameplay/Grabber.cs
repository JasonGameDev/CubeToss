using System;
using System.Collections.Generic;
using CubeToss.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CubeToss.Gameplay
{
    public class Grabber : MonoBehaviour
    {
        [Header("Camera Reference")]
        [SerializeField] private Camera mainCamera;
        
        [Header("Grab Settings")]
        [SerializeField] private float grabDistance = 0.3f;
        [SerializeField] private float holdDistance = 0.1f;
        [SerializeField] private float grabSpeed = 5f;
        [SerializeField] private float positionError = 0.01f;

        [Header("Flick Settings")]
        [SerializeField, Range(1, 60)] private int detectionSampleCount = 3;
        [SerializeField] private float flickThreshold = 200.0f;
        [SerializeField] private float throwMultiplier = 0.01f;          
        [SerializeField] private float horizontalScale = 1.0f;
        [SerializeField] private float verticalScale = 1.0f;
        [SerializeField] private float forwardScale = 1.0f;
        [SerializeField] private float maxVerticalVelocity = 10.0f;
        
        // TODO: lets move to an input action reference so we can use the inspector to assign the input action asset
        private CubeToss_InputActions _inputActions;

        private GrabbableObject _currentGrabbable;

        private readonly Queue<Sample> _flickDetectionWindow = new();
        private Sample? _flickStartSample;
        
        private struct Sample
        {
            public readonly Vector2 Position;
            public readonly float Time;

            public Sample(Vector2 position, float time) { Position = position; Time = time; }
        }
        
        private void Awake()
        {
            if(!mainCamera)
                mainCamera = Camera.main;
            
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

            switch (_currentGrabbable.State)
            {
                case GrabbableObject.GrabState.Idle:
                    _currentGrabbable.StartGrab(holdPoint, grabSpeed);
                    
                    _flickDetectionWindow.Clear();
                    _flickStartSample = null;
                    break;
                
                case GrabbableObject.GrabState.Grabbing:
                    _currentGrabbable.UpdateGrab(holdPoint, grabSpeed);
                    
                    var distance = Vector3.Distance(_currentGrabbable.transform.position, holdPoint);
                    if (distance <= positionError)
                        _currentGrabbable.CompleteGrab();
                    break;
                
                case GrabbableObject.GrabState.Held:
                    _currentGrabbable.HoldGrab(holdPoint);
                    
                    _flickDetectionWindow.Enqueue(new Sample(pointerPos, Time.time));
                    if (_flickDetectionWindow.Count > detectionSampleCount)
                        _flickDetectionWindow.Dequeue();

                    if (_flickDetectionWindow.Count == detectionSampleCount)
                    {
                        var firstSample = _flickDetectionWindow.Peek();
                        var lastSample = default(Sample);
                        foreach (var sample in _flickDetectionWindow)
                            lastSample = sample;

                        var deltaTime = lastSample.Time - firstSample.Time;
                        if (deltaTime > 0.0f)
                        {
                            var speed = (lastSample.Position - firstSample.Position).magnitude / deltaTime;
                            if (_flickStartSample == null && speed > flickThreshold)
                                _flickStartSample = lastSample;
                            else if (speed < flickThreshold)
                                _flickStartSample = null;
                        }
                    }
                    break;
            }
        }

        private void OnGrabStarted(InputAction.CallbackContext ctx)
        {
            var pointer = _inputActions.Player.PointerPosition.ReadValue<Vector2>();
            var ray = mainCamera.ScreenPointToRay(pointer);

            if (Physics.Raycast(ray, out RaycastHit hit, grabDistance) && hit.collider.TryGetComponent(out GrabbableObject grabbable))
            {
                _currentGrabbable = grabbable;
                _flickDetectionWindow.Clear();
                _flickStartSample = null;
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
                    if (_flickStartSample.HasValue)
                    {
                        var startSample = _flickStartSample.Value;
                        var releaseSample = new Sample(_inputActions.Player.PointerPosition.ReadValue<Vector2>(), Time.time);
                        
                        var deltaPosition = releaseSample.Position - startSample.Position;
                        var deltaTime = Mathf.Max(0.001f, releaseSample.Time - startSample.Time);
                        var screenVelocity = deltaPosition / deltaTime;

                        var worldDirection =
                            (Vector3.right * (screenVelocity.x * horizontalScale) +
                             Vector3.up * (screenVelocity.y * verticalScale) +
                             Vector3.forward * (screenVelocity.y * forwardScale)).normalized;

                        var velocity = worldDirection * (screenVelocity.magnitude * throwMultiplier);
                        velocity.y = Mathf.Clamp(velocity.y, -maxVerticalVelocity, maxVerticalVelocity);
                        _currentGrabbable.ReleaseGrab(velocity);
                    }
                    else
                    {
                        _currentGrabbable.CancelGrab();
                    }
                    break;
            }

            _currentGrabbable = null;
            _flickDetectionWindow.Clear();
            _flickStartSample = null;
        }
    }
}
