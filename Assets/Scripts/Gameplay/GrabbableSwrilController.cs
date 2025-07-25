using System;
using UnityEngine;
using CubeToss.Events;
using Random = UnityEngine.Random;

namespace CubeToss.Gameplay
{
    [RequireComponent(typeof(GrabbableObject))]
    public class GrabbableSwirlController : MonoBehaviour
    {
        [SerializeField]private GrabbableEventChannel eventChannel;
        
        [SerializeField] private float minSpin = 60f;
        [SerializeField] private float maxSpin = 120f;
        
        private float _theta;
        private float _phi;
        private float _radius;
        private float _swirlSpeed;
        private Vector3 _center;
        private GrabbableObject _grabbable;

        private Vector3 _spinDegreesPerSecond;

        private void Awake()
        {
            _grabbable = GetComponent<GrabbableObject>();
        }

        public void Init(float theta, float phi, float radius, float swirlSpeed, Vector3 center)
        {
            _theta = theta;
            _phi = phi;
            _radius = radius;
            _swirlSpeed = swirlSpeed;
            _center = center;

            _spinDegreesPerSecond = Random.onUnitSphere * Random.Range(minSpin, maxSpin);

            eventChannel.GrabStarted.AddListener(OnStartedGrab);
            eventChannel.GrabCanceled.AddListener(OnCanceledGrab);

            UpdatePosition();
        }

        private void Update()
        {
            if (_grabbable.State == GrabbableObject.GrabState.Idle)
            {
                _theta += _swirlSpeed * Mathf.Deg2Rad * Time.deltaTime * 0.2f;
                _phi += _swirlSpeed * Mathf.Deg2Rad * Time.deltaTime * 0.1f;
                UpdatePosition();
            }
        }

        private void FixedUpdate()
        {
            if (_grabbable.State == GrabbableObject.GrabState.Idle)
            {
                transform.Rotate(_spinDegreesPerSecond * Time.fixedDeltaTime, Space.World);
            }
        }

        private void UpdatePosition()
        {
            _theta = Mathf.Repeat(_theta, 2 * Mathf.PI);
            _phi = Mathf.Repeat(_phi, 2 * Mathf.PI);

            var x = Mathf.Sin(_phi) * Mathf.Cos(_theta);
            var y = Mathf.Sin(_phi) * Mathf.Sin(_theta);
            var z = Mathf.Cos(_phi);
            transform.position = _center + new Vector3(x, y, z) * _radius;
        }

        private void OnDisable()
        {
            eventChannel.GrabStarted.RemoveListener(OnStartedGrab);
            eventChannel.GrabCanceled.RemoveListener(OnCanceledGrab);
        }

        private void OnStartedGrab(GrabbableObject grabbable) { }
        private void OnCanceledGrab(GrabbableObject grabbable) { }
    }
}
