using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace CubeToss.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class Asteroid : MonoBehaviour
    {
        [Header("Return to Kinematic Settings")]
            
        [SerializeField] private float minActiveTime = 1.0f;
        [SerializeField] private float maxActiveTime = 3.0f;
        [SerializeField] private float initialSpinMin = 60.0f;
        [SerializeField] private float initialSpinMax = 120.0f;
        [SerializeField] private float velocityThreshold = 1.0f;
        [SerializeField] private float brakeSpeed = 0.01f;
        [SerializeField] private float rotationMax = 180.0f;
        [SerializeField] private float brakeRotation = 10.0f;
        
        private Rigidbody _rigidbody;
        private Collider _collider;
    
        private Vector3 _spinDegreesPerSecond;
        private Vector3 _spinRadiansPerSecond;
        private Vector3 _residualVelocity;
        
        private float _timer;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            _spinDegreesPerSecond = Random.onUnitSphere * Random.Range(initialSpinMin, initialSpinMax);
            _spinRadiansPerSecond = _spinDegreesPerSecond * Mathf.Deg2Rad;
            
            _rigidbody.isKinematic = true;
            _collider.isTrigger = true;
        }

        private void FixedUpdate()
        {
            if (_rigidbody.isKinematic)
            {
                if (_spinDegreesPerSecond.magnitude > rotationMax)
                    _spinDegreesPerSecond = _spinDegreesPerSecond.normalized * rotationMax;
                
                transform.Rotate(_spinDegreesPerSecond * Time.fixedDeltaTime, Space.World);
                
                if (_spinDegreesPerSecond.magnitude > initialSpinMax)
                {
                    _spinDegreesPerSecond = Vector3.MoveTowards(_spinDegreesPerSecond,
                        _spinDegreesPerSecond.normalized * initialSpinMax, brakeRotation * Time.fixedDeltaTime);
                }
                
                _spinRadiansPerSecond = _spinDegreesPerSecond * Mathf.Deg2Rad;
                
                if(_residualVelocity.magnitude > 0.0f)
                {
                    var position = _rigidbody.position + _residualVelocity * Time.fixedDeltaTime;
                    _rigidbody.MovePosition(position);
                    
                    _residualVelocity = Vector3.MoveTowards(_residualVelocity, Vector3.zero, brakeSpeed * Time.fixedDeltaTime);
                    
                    if (_residualVelocity.magnitude < 0.01f)
                        _residualVelocity = Vector3.zero;
                }
            }
            else
            {
                var maxSpinRadiansPerSecond = rotationMax * Mathf.Deg2Rad;
                var spin = _rigidbody.angularVelocity;
                if(spin.magnitude > maxSpinRadiansPerSecond)
                    _rigidbody.angularVelocity = spin.normalized * maxSpinRadiansPerSecond;
                
                _timer += Time.fixedDeltaTime;
                if (_timer > minActiveTime)
                {
                    if (_timer > maxActiveTime || _rigidbody.linearVelocity.magnitude < velocityThreshold)
                    {
                        _spinRadiansPerSecond = _rigidbody.angularVelocity;
                        _spinDegreesPerSecond = _spinRadiansPerSecond * Mathf.Rad2Deg;
                        
                        _residualVelocity = _rigidbody.linearVelocity;
                        
                        _rigidbody.isKinematic = true;
                        _collider.isTrigger = true;
                        _timer = 0f;
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_rigidbody.isKinematic)
                return;

            _collider.isTrigger = false;
            _rigidbody.isKinematic = false;
            _timer = 0f;
            
            _rigidbody.AddTorque(_spinRadiansPerSecond, ForceMode.VelocityChange);
        }
    }
}