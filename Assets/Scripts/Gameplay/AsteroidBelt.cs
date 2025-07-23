using UnityEngine;

namespace CubeToss.Gameplay
{
    public class AsteroidBelt : MonoBehaviour
    {
        [SerializeField] private Asteroid asteroidPrefab;

        [SerializeField] private int count = 100;
        [SerializeField] private float innerRadius = 20f;
        [SerializeField] private float outerRadius = 30f;
        [SerializeField] private float beltHeight = 2f;

        void Start()
        {
            for (int i = 0; i < count; i++)
            {
                var radius = Random.Range(innerRadius, outerRadius);
                var angle = Random.Range(0, 360) * Mathf.Deg2Rad;
                var position = transform.position +
                               new Vector3(radius * Mathf.Cos(angle),
                                   Random.Range(-beltHeight, beltHeight),
                                   radius * Mathf.Sin(angle));

                Instantiate(asteroidPrefab, position, Quaternion.identity, transform);
            }
        }
    }
}