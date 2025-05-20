using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Settings")]
    public float minSpawnDelay; 
    public float maxSpawnDelay; 
    [Header("References")]
    public GameObject[] gameObjects;
    void OnEnable()
    {
        Invoke("spawn",  Random.Range(minSpawnDelay, maxSpawnDelay));
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void spawn()
    {
        GameObject randomObject = gameObjects[Random.Range(0, gameObjects.Length)];    
        Instantiate(randomObject, transform.position, Quaternion.identity);
        Invoke("spawn",  Random.Range(minSpawnDelay, maxSpawnDelay));
    }
}
