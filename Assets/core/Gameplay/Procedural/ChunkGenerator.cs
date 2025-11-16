using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment;
using Core;
using System.Threading.Tasks; // for ProceduralSettings

public class ChunkGenerator : MonoBehaviour
{
    [Header("Chunk Grid Settings")]
    [SerializeField] private int usableCols = 10;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float chunkHeight = 50f;

    [Header("References")]
    [SerializeField] private Transform hazardsParent; // will hold all spawned hazards
    private float difficulty = 0f;   // current global difficulty (0–1)

    private ProceduralSettings settings;

    private void Awake()
    {
        settings = GameManager.Instance.proceduralSettings;
        if (settings == null)
        {
            Debug.LogError("ProceduralSettings not assigned in GameManager.");
        }
    }
    private async void Start()
    {
        hazardsParent = transform;
        await Task.Delay(100);
        difficulty = GameManager.Instance.difficulty;
        StartCoroutine(GenerateChunkCoroutine(transform.position.y-25));
        Debug.Log(transform.position.y - 25);
    }

    public IEnumerator GenerateChunkCoroutine(float baseY)
    {
        if (settings == null)
        {
            Debug.LogError("Cannot generate chunk: Missing ProceduralSettings reference.");
            yield break;
        }

        Random.InitState(System.Environment.TickCount ^ GetInstanceID());

        // Build grid
        float originX = -(usableCols * cellSize) / 2f;
        Vector2 origin = new Vector2(originX, baseY);
        var grid = new ChunkGrid(origin, usableCols, Mathf.CeilToInt(chunkHeight / cellSize), cellSize);

        List<System.Action> spawners = HazardChunkSpawner.BuildSpawnerList(
            grid,
            hazardsParent,
            difficulty,
            settings
        );

        // Shuffle to avoid fixed order patterns
        Shuffle(spawners);

        // Iterate through spawn actions
        foreach (var spawnAction in spawners)
        {
            yield return new WaitForSeconds(0.1f);

            spawnAction?.Invoke();
            yield return null; 
        }

        //Debug.Log($"Chunk at Y={baseY} generated with {spawners.Count} hazard groups.");
    }

    /// Utility to randomize list order.
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }
}
