using UnityEngine;

namespace CubeToss.Environment
{
    public class SimpleRotate : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 30, 0);

        private void FixedUpdate()
        {
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime, Space.World);
        }
    }
}