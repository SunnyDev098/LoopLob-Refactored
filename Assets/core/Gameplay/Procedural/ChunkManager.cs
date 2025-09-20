using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [System.Serializable]
    public class ChunkTier
    {
        public string name;
        public GameObject[] chunkPrefabs;
    }

    [Header("Chunk System Settings")]
    public ChunkTier[] chunkTiers;
    public float chunkHeight = 200f;

    private Vector3 nextSpawnPosition;
    private int spawnedCount;
    private int[] lastPrefabIndexPerTier;
    
    private void Awake()
    {
        ResetSpawner();
    }

    public void ResetSpawner()
    {
        spawnedCount = 0;
        nextSpawnPosition = Vector3.zero;

        int tierCount = chunkTiers != null ? chunkTiers.Length : 0;
        lastPrefabIndexPerTier = new int[tierCount];

        for (int i = 0; i < tierCount; i++)
            lastPrefabIndexPerTier[i] = -1;
    }
    
    public void SpawnFirstChunk(Transform chunkParent)
    {
        SpawnChunkFromTier(0, chunkParent);
    }

    public void SpawnNextChunk(float playerY, Transform chunkParent)
    {
        int tierIndex = DecideTierIndex(playerY);
        SpawnChunkFromTier(tierIndex, chunkParent);
    }

    private int DecideTierIndex(float playerY)
    {
        if (chunkTiers == null || chunkTiers.Length == 0)
            return 0;

        if (playerY < 1000f) return 0;
        else if (playerY < 2000f) return Mathf.Min(1, chunkTiers.Length - 1);
        else if (playerY < 3000f) return Mathf.Min(2, chunkTiers.Length - 1);
        else return chunkTiers.Length - 1;
    }

    private void SpawnChunkFromTier(int tierIndex, Transform chunkParent)
    {
        if (lastPrefabIndexPerTier == null || lastPrefabIndexPerTier.Length != chunkTiers.Length)
        {
            Debug.LogWarning("ChunkManager not initied ");
            ResetSpawner();
        }

        if (chunkTiers == null || chunkTiers.Length == 0)
        {
            Debug.LogWarning("ChunkManager  No tiers assigned");
            return;
        }

        if (tierIndex >= chunkTiers.Length)
            tierIndex = chunkTiers.Length - 1;

        ChunkTier tier = chunkTiers[tierIndex];
        if (tier.chunkPrefabs == null || tier.chunkPrefabs.Length == 0)
        {
            Debug.LogWarning($"ChunkManager Tier  has no prefabs");
            return;
        }

        int prefabIndex;
        if (tier.chunkPrefabs.Length == 1)
        {
            prefabIndex = 0;
        }
        else
        {
            do
            {
                prefabIndex = Random.Range(0, tier.chunkPrefabs.Length);
            } while (prefabIndex == lastPrefabIndexPerTier[tierIndex]);
        }

        lastPrefabIndexPerTier[tierIndex] = prefabIndex;

        Instantiate(tier.chunkPrefabs[prefabIndex], nextSpawnPosition, Quaternion.identity, chunkParent);
        nextSpawnPosition.y += chunkHeight;
        spawnedCount++;
    }
}
