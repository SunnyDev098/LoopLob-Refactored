using UnityEngine;
using System.Collections.Generic;

public class BackGroundObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<Sprite> spritesToSpawn;
    public GameObject platformPrefab;
    public Transform parentContainer;
    public GameObject ball;

    [Tooltip("Distance in Y before new spawn check.")]
    public float spawnDistanceGap = 250f;

    public bool startOnAwake = true;

    private float nextSpawnY;
    private int currentIndex = 0;
    private const int MaxObjects = 20;

    private void Start()
    {
        nextSpawnY = 50;
        if (ball == null)
        {
            Debug.LogError("ObjectSpawner requires a reference to the ball GameObject.");
            enabled = false;
            return;
        }

        if (startOnAwake && spritesToSpawn.Count > 0)
        {
            ScheduleNextSpawn(ball.transform.position.y + spawnDistanceGap);
        }
    }
    // spawning Big background objects and moving them with ball vertical movment
    private void Update()
    {
        if (currentIndex >= MaxObjects || spritesToSpawn.Count == 0)
            return;

        if (ball.transform.position.y >= nextSpawnY)
        {
            SpawnNextObject();
        }

        if (currentIndex > 0)
        {
            transform.position = new Vector3(0, ball.transform.position.y * 0.8f, -10);

        }


    }

    private void SpawnNextObject()
    {
        // Random horizontal position
        float randomX = Random.Range(-3f, 3f);
        Vector3 spawnPos = new Vector3(randomX, ball.transform.position.y +50, 30f);

        // Instantiate platform
        GameObject newPlatform = Instantiate(platformPrefab, spawnPos, Quaternion.identity, parentContainer);

        // Random scale multiplier
        float scaleMultiplier = Random.Range(0.7f, 1f);
        newPlatform.transform.localScale *= scaleMultiplier;

        // Assign sprite
        newPlatform.GetComponent<SpriteRenderer>().sprite = spritesToSpawn[currentIndex];

        // Prepare for next spawn
        currentIndex++;
        ScheduleNextSpawn(ball.transform.position.y + currentIndex * spawnDistanceGap);
    }

    private void ScheduleNextSpawn(float yPosition)
    {
        if (currentIndex == 0)
        {
            nextSpawnY = yPosition ;

        }
        else
        {
            nextSpawnY = yPosition + 250;

        }
    }
}
