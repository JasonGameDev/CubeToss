using System;
using CubeToss.Events;
using CubeToss.Gameplay;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CubeToss.Cameras
{
    public class CameraManager : CinemachineCameraManagerBase
    {
        [SerializeField] private CinemachineCamera stationaryCamera;
        [SerializeField] private CinemachineCamera followZoomCamera;
        [SerializeField] private CinemachineCamera followCamera;
        [SerializeField] private CinemachineCamera lookAtCamera;
        
        [SerializeField] private float timeOutDuration = 3.0f;
        
        [SerializeField] private GrabbableEventChannel grabbableEventChannel;

        private CinemachineCamera _currentCamera;

        private float _timer;
        private bool TimeOut
        {
            get
            {
                if(_timer < 0.0f)
                    return false;
                
                if(Time.time > _timer)
                {
                    _timer = -1.0f;
                    return true;
                }

                return false;
            }
            set => _timer = value ? Time.time + timeOutDuration : -1.0f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            grabbableEventChannel.GrabReleased.AddListener(OnGrabbableReleased);
            grabbableEventChannel.EnteredPlane.AddListener(OnEnteredPlane);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            grabbableEventChannel.GrabReleased.RemoveListener(OnGrabbableReleased);
            grabbableEventChannel.EnteredPlane.RemoveListener(OnEnteredPlane);
        }

        protected override void Start()
        {
            base.Start();
            
            _currentCamera = stationaryCamera;
            TimeOut = false;
        }

        protected override void Update()
        {
            base.Update();
            
            if (_currentCamera != stationaryCamera && TimeOut)
                _currentCamera = stationaryCamera;
        }

        protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
        {
            return _currentCamera;
        }

        private void OnGrabbableReleased(GrabbableObject grabbable)
        {
            followCamera.Target.TrackingTarget = grabbable.transform;
            followCamera.enabled = true;
            
            _currentCamera = followCamera;
            TimeOut = true;
        }

        private void OnEnteredPlane(GrabbableObject grabbable)
        {
            // TODO: Need to figure out camera cut bug
            
            lookAtCamera.transform.position = followCamera.transform.position;
            lookAtCamera.transform.rotation = followCamera.transform.rotation;

            followCamera.Target.TrackingTarget = null;
            
            _currentCamera = lookAtCamera;
            TimeOut = true;
        }
    }
}
