using System;
using UnityEngine;
using System.Collections.Generic;
using CubeToss.Events;
using Random = UnityEngine.Random;

namespace CubeToss.Gameplay
{
    public class GrabbableSpawner : MonoBehaviour
    {
        [SerializeField] private GrabbableEventChannel eventChannel;

        [SerializeField] private GameObject grabbablePrefab;

        [SerializeField] private int initialCount = 10;
        [SerializeField] private float radius = 200f;
        [SerializeField] private float swirlSpeed = 500f;

        private readonly Dictionary<GrabbableObject, GrabbableSwirlController> _lookup = new();

        private void OnEnable()
        {
            eventChannel.GrabReleased.AddListener(OnGrabReleased);
        }

        private void OnDisable()
        {
            eventChannel.GrabReleased.RemoveListener(OnGrabReleased);
        }

        private void Start()
        {
            for (int i = 0; i < initialCount; i++)
                SpawnGrabbable(i);
        }

        private void OnGrabReleased(GrabbableObject grabbable)
        {
            if (!_lookup.ContainsKey(grabbable)) 
                return;
            
            _lookup.Remove(grabbable);
            SpawnGrabbable(Random.Range(0, int.MaxValue));
        }

        private void SpawnGrabbable(int seed)
        {
            var rand = new System.Random(seed);
            var theta = (float)(rand.NextDouble() * Mathf.PI * 2f);
            var phi = (float)(rand.NextDouble() * Mathf.PI);

            var grabbable = Instantiate(grabbablePrefab, transform.position, Random.rotation, transform).GetComponent<GrabbableObject>();
            var swirl = grabbable.GetComponent<GrabbableSwirlController>();

            swirl.Init(theta, phi, radius, swirlSpeed, transform.position);
            _lookup[grabbable] = swirl;
        }
    }
}