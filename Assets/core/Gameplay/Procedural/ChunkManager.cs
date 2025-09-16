using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    [Tooltip("All chunk prefabs designed by level designers.")]
    public List<GameObject> chunkPrefabs;

    [Tooltip("Vertical distance each chunk covers in world Y units.")]
    public float chunkHeight = 50f;

    [Tooltip("Number of chunks to keep loaded at any time.")]
    public int chunksVisible = 5;

    [Tooltip("How far below the player to remove old chunks.")]
    public float cleanupBuffer = 30f;

    [Header("References")]
    public Transform player; // Ball transform
    public Transform chunkParent; // Optional parent for spawned chunks

    private Queue<GameObject> activeChunks = new Queue<GameObject>();
    private float nextSpawnY;
    private float lastPlayerY;

    private void Start()
    {
        nextSpawnY = 0f;
        lastPlayerY = player.position.y;
        for (int i = 0; i < chunksVisible; i++)
        {
            SpawnNextChunk();
        }
    }

    private void Update()
    {
        if (player.position.y > lastPlayerY)
        {
            if (player.position.y + chunkHeight * (chunksVisible / 2) > nextSpawnY)
            {
                SpawnNextChunk();
                CleanupOldChunks();
            }
        }
        lastPlayerY = player.position.y;
    }

    private void SpawnNextChunk()
    {
        if (chunkPrefabs.Count == 0) return;

        GameObject prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Count)];
        GameObject chunk = Instantiate(prefab, new Vector3(0, nextSpawnY, 0), Quaternion.identity, chunkParent);
        activeChunks.Enqueue(chunk);
        nextSpawnY += chunkHeight;
    }

    private void CleanupOldChunks()
    {
        while (activeChunks.Count > chunksVisible)
        {
            GameObject oldChunk = activeChunks.Dequeue();
            if (oldChunk)
                Destroy(oldChunk);
        }
    }
}
