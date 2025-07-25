using UnityEngine;

namespace CubeToss.Environment
{
    public class Orbit : MonoBehaviour
    {
        [SerializeField] private float orbitSpeed = 10;
        [SerializeField] private float orbitRadius = 500f;
        [SerializeField] private Vector3 orbitAxis = Vector3.up;

        private float _angle;

        private void Start()
        {
            var offset = transform.position - Vector3.zero;
            _angle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg;
        }

        private void Update()
        {
            _angle += orbitSpeed * Time.deltaTime;
            
            var rad = _angle * Mathf.Deg2Rad;
            var newPos = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * orbitRadius;
            
            transform.position = newPos;
        }
    }
}