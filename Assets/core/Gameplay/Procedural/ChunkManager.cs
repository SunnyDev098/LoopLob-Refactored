using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk System Settings")]
    [SerializeField] public GameObject chunkPrefab;  // Single chunk prefab for spawning
    [SerializeField] private float chunkHeight = 50f; // Height of each chunk
    [SerializeField] private float spawnThresholdDistance = 50f; // Distance the player must move to spawn a new chunk

    [Header("References")]
    [SerializeField] private Transform ball;           // Reference to the ball/player
    [SerializeField] private Transform chunksParent;   // Parent to keep hierarchy clean

    [SerializeField] private float yOfsset =10;
    private Vector3 nextSpawnPosition;  // The next spawn position for the chunk
    private float lastSpawnY;           // Last Y position of the ball to track distance

    private void Start()
    {
        lastSpawnY = ball.position.y;
        // Spawn the first chunk immediately
        SpawnChunk();
    }

    private void Update()
    {
        if (!ball) return;

        float playerY = ball.position.y;

        // Check if player has moved far enough to spawn a new chunk
        if (playerY - lastSpawnY  >= spawnThresholdDistance)
        {
            SpawnChunk();
            lastSpawnY = playerY;
        }
    }

    // Spawn a chunk at the next spawn position
    private void SpawnChunk()
    {
        Debug.Log("someChunk Just Maded!");
        // Instantiate the chunk at the next spawn position
       GameObject madedChunk = Instantiate(chunkPrefab, nextSpawnPosition, Quaternion.identity, chunksParent);
       // madedChunk.GetComponent<PlanetGenerator>().difficulty = ball.position.y / 5000;
        madedChunk.GetComponent<PlanetGenerator>().difficulty = 1;
        madedChunk.GetComponent<PlanetGenerator>().GenerateCaller(ball.position.y+ yOfsset, madedChunk.transform) ;
        // Update the next spawn position by adding chunkHeight to the Y value
        nextSpawnPosition.y += chunkHeight;
    }
}
