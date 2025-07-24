using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class TestCubeSpawner : MonoBehaviour
{
    [System.Serializable]
    private class CubeData
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public GameObject prefab;
    }

    [SerializeField, HideInInspector] private List<CubeData> originalCubes = new List<CubeData>();

    void Start()
    {
        originalCubes.Clear();
        foreach (Transform child in transform)
        {
            var cubeData = new CubeData
            {
                localPosition = child.localPosition,
                localRotation = child.localRotation,
                prefab = Instantiate(child.gameObject),
            };
            
            cubeData.prefab.SetActive(false);
            originalCubes.Add(cubeData);
        }
    }

    public void RespawnCubes()
    {
        foreach (var cubeData in originalCubes)
        {
            var newCube = Instantiate(cubeData.prefab, transform);
            newCube.transform.localPosition = cubeData.localPosition;
            newCube.transform.localRotation = cubeData.localRotation;
            newCube.SetActive(true);
        }
    }
}

[CustomEditor(typeof(TestCubeSpawner))]
public class TestCubeSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TestCubeSpawner spawner = (TestCubeSpawner)target;
        if (GUILayout.Button("Respawn Cubes"))
        {
            spawner.RespawnCubes();
        }
    }
}