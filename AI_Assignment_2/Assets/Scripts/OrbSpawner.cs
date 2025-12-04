using UnityEditor;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    [SerializeField] float spawnCooldown;
    [SerializeField] GameObject prefab;
    [SerializeField] BoxCollider boundingBox;
    [SerializeField] bool isActive;
    float spawnTimer;
    void Start()
    {
        spawnTimer = spawnCooldown;
    }

    void Update()
    {
        if (!isActive) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer < 0)
        {
            float x = Random.Range(boundingBox.bounds.min.x, boundingBox.bounds.max.x);
            float z = Random.Range(boundingBox.bounds.min.z, boundingBox.bounds.max.z);

            Instantiate(prefab, new Vector3(x, 1, z), Quaternion.identity, this.transform);

            spawnTimer = spawnCooldown;
        }
    }
}
