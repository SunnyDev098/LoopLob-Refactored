using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BackGroundObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<Sprite> spritesToSpawn;
    public GameObject platformPrefab;
    public Transform parentContainer;
    public GameObject camera;

    [Tooltip("Distance in Y before new spawn check.")]
    public float spawnDistanceGap = 250f;

    public bool startOnAwake = true;

    [Header("Distance Banner Settings")]
    public GameObject distanceBannerPrefab;
    public Transform bannerParentContainer;

    [Tooltip("Y interval for distance banners (default 500).")]
    public float bannerInterval = 500f;

    private float nextSpawnY;
    private int currentIndex = 0;
    private const int MaxObjects = 20;

    // Banner tracking
    private float nextBannerTriggerY = 400f; // First trigger at Y=400
    private int bannerCount = 0;
    private float timer;
    private const float interval = 1f;
    private void Start()
    {
        /*
        nextSpawnY = 50;
        if (camera == null)
        {
            Debug.LogError("ObjectSpawner requires a reference to the cam GameObject.");
            enabled = false;
            return;
        }

        if (startOnAwake && spritesToSpawn.Count > 0)
        {
            ScheduleNextSpawn(camera.transform.position.y + spawnDistanceGap);
        }
        */
    }

    // spawning Big background objects and moving them with ball vertical movement
    private void Update()
    {
       
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                timer = 0f;
                CheckAndSpawnDistanceBanner();
            }
        
        /*
        if (currentIndex >= MaxObjects || spritesToSpawn.Count == 0)
            return;

        if (camera.transform.position.y >= nextSpawnY)
        {
            SpawnNextObject();
        }

        if (currentIndex > 0)
        {
            transform.position = new Vector3(0, camera.transform.position.y * 0.8f, -10);
        }
        */
    }

    /// <summary>
    /// Checks if camera has reached a banner trigger point and spawns banner if needed.
    /// Trigger points: 400, 900, 1400, 1900, ... (Y = 500n - 100 where n = 1, 2, 3, ...)
    /// Banner spawns at: 500, 1000, 1500, 2000, ... (Y = 500n where n = 1, 2, 3, ...)
    /// </summary>
    private void CheckAndSpawnDistanceBanner()
    {
        if (distanceBannerPrefab == null)
            return;

        // Check if camera has crossed the trigger threshold
        if (camera.transform.position.y >= nextBannerTriggerY)
        {
            bannerCount++;

            // Calculate banner Y position (500, 1000, 1500, ...)
            float bannerY = bannerCount * bannerInterval;

            // Spawn banner at (0, bannerY, 0)
            Vector3 bannerPosition = new Vector3(0f, bannerY, -10f);

            Transform parent = bannerParentContainer != null ? bannerParentContainer : transform;
            GameObject banner = Instantiate(distanceBannerPrefab, bannerPosition, Quaternion.identity);
            banner.name = $"DistanceBanner_{bannerCount * 500}m";
            float nextMark = Mathf.Ceil(camera.transform.position.y / 500f) * 500f;
            banner.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = nextMark.ToString("0") + " m";            // Update next trigger point (400, 900, 1400, ...)
            nextBannerTriggerY = (bannerCount * bannerInterval) + (bannerInterval - 100f);

            Debug.Log($"Spawned distance banner at Y={bannerY} (trigger was Y={nextBannerTriggerY - bannerInterval + 100})");
        }
    }

    private void SpawnNextObject()
    {
        // Random horizontal position
        float randomX = Random.Range(-3f, 3f);
        Vector3 spawnPos = new Vector3(randomX, camera.transform.position.y + 50, 30f);

        // Instantiate platform
        GameObject newPlatform = Instantiate(platformPrefab, spawnPos, Quaternion.identity, parentContainer);

        // Random scale multiplier
        float scaleMultiplier = Random.Range(0.7f, 1f);
        newPlatform.transform.localScale *= scaleMultiplier;

        // Assign sprite
        newPlatform.GetComponent<SpriteRenderer>().sprite = spritesToSpawn[currentIndex];

        // Prepare for next spawn
        currentIndex++;
        ScheduleNextSpawn(camera.transform.position.y + currentIndex * spawnDistanceGap);
    }

    private void ScheduleNextSpawn(float yPosition)
    {
        if (currentIndex == 0)
        {
            nextSpawnY = yPosition;
        }
        else
        {
            nextSpawnY = yPosition + 250;
        }
    }
}
