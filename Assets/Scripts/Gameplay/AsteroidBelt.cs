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
                var position = transform.position + new Vector3(radius * Mathf.Cos(angle),
                    Random.Range(-beltHeight, beltHeight), radius * Mathf.Sin(angle));

                var rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                var asteroid = Instantiate(asteroidPrefab, position, rotation, transform);
                
                asteroid.transform.localScale *= Random.Range(0.5f, 1.5f);
                
                var rb = asteroid.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.mass = asteroid.transform.localScale.x;
                }
            }
        }
    }
}