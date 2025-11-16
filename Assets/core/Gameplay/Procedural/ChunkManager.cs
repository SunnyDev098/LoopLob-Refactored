using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    [Header("Chunk System Settings")]
    private float lastChunkBottom;
    [SerializeField] public List<Chunk> chunkPrefabs;
    [SerializeField] public List<Chunk> FirstChunkPrefabs;
    [SerializeField] public List<GameObject> LoadedChunks; 
    [SerializeField] private float chunkHeight = 50f; // Height of each chunk
    private float spawnThresholdDistance = 30f; // Distance the player must move to spawn a new chunk

    [Header("References")]
    [SerializeField] private Transform ball;           // Reference to the ball/player
    [SerializeField] private Transform chunksParent;   // Parent to keep hierarchy clean

    private Vector3 nextSpawnPosition;  // The next spawn position for the chunk
    private float lastSpawnY;           // Last Y position of the ball to track distance

    private void Start()
    {
        lastSpawnY = ball.position.y- FirstChunkPrefabs[0].height;
        SpawnChunk(FirstChunkPrefabs[Random.RandomRange(0, FirstChunkPrefabs.Count)]);
    }

    private void Update()
    {
        if (!ball) return;

        float playerY = ball.position.y;

       // if (playerY - lastSpawnY  >= spawnThresholdDistance)
        if (playerY >= lastChunkBottom- spawnThresholdDistance)
        {
            spawnNextChunk();
            lastSpawnY = playerY;

            if(LoadedChunks.Count>1  && LoadedChunks[0].GetComponent<Chunk>().getTop()<playerY - spawnThresholdDistance)
            {
             
                DestroyChunk(LoadedChunks[0]);
            }

        }

    }
    private void spawnNextChunk()
    {
        var chunk = chunkPrefabs[Random.RandomRange(0, chunkPrefabs.Count)];
        SpawnChunk(chunk);



    }


    private void SpawnChunk(Chunk prefab)
    {
      //  Debug.Log("someChunk Just Maded!");
        GameObject madedChunk = Instantiate(prefab.gameObject, nextSpawnPosition, Quaternion.identity, chunksParent);
        nextSpawnPosition.y += chunkHeight;


        LoadedChunks.Add(madedChunk);
        lastChunkBottom = madedChunk.GetComponent<Chunk>().getBottom();
    }

    private void DestroyChunk(GameObject chunk)
    {
        LoadedChunks.Remove(chunk);
        Destroy(chunk.gameObject);

    }
}
