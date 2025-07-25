using System.Collections;
using UnityEngine;

namespace CubeToss.Gameplay
{
    // TODO: Optimize asteroid generation by using burst/jobs to generate the asteroid info in parallel,
    // TODO: Bonus: Switch to ECS.
    public class AsteroidBelt : MonoBehaviour
    {
        [SerializeField] private Asteroid asteroidPrefab;

        [SerializeField] private int count = 1000;
        [SerializeField] private int batchSize = 100;
        [SerializeField] private float innerRadius = 150.0f;
        [SerializeField] private float outerRadius = 400.0f;
        [SerializeField] private float beltHeight = 50.0f;
        [SerializeField] private float rotationSpeed = 25;

        private void Start()
        {
            StartCoroutine(SpawnAsteroids());
        }

        private void FixedUpdate()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime, Space.World);
        }

        private IEnumerator SpawnAsteroids()
        {
            for (int i = 0; i < count; i += batchSize)
            {
                for (int j = 0; j < batchSize && i + j < count; j++)
                {
                    var radius = Random.Range(innerRadius, outerRadius);
                    var angle = Random.Range(0, 360) * Mathf.Deg2Rad;
                    var position = transform.position + new Vector3(radius * Mathf.Cos(angle),
                        Random.Range(-beltHeight, beltHeight), radius * Mathf.Sin(angle));

                    var rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                    var asteroid = Instantiate(asteroidPrefab, position, rotation, transform);
                    
                    asteroid.transform.localScale *= Random.Range(0.5f, 1.5f);
                    asteroid.gameObject.layer = gameObject.layer;
                    
                    var rb = asteroid.GetComponent<Rigidbody>();
                    if (rb)
                    {
                        rb.mass = asteroid.transform.localScale.x * 0.1f;
                    }
                }
                
                yield return null;
            }
        }
    }
}