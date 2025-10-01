using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Difficulty Settings")]
    [Range(0f, 1f)]
    private float difficulty;

    [Header("Chunk Bounds")]
    public float chunkHeight = 50f;
    public float chunkWidth = 10f;
    public float cellSize = 1f;
    public int usableCols = 8;

    [Header("Prefabs")]
    public GameObject planetPrefab;
    public GameObject spikePrefab;
    public GameObject blackHolePrefab;
    public GameObject laserGunPrefab;
    public GameObject alienShipPrefab;
    public GameObject missilePrefab;
    public GameObject beamEmitterPrefab;

    [Header("Difficulty Thresholds")]
    private float blackHoleThreshold = 0.06f;
    private float alienShipThreshold = 0.1f;
    private float beamEmitterThreshold = 0.2f;
    private float laserGunThreshold = 0.4f;
    private float rocketLauncherThreshold = 0.5f;
    private float badPlanetThreshold = 0.7f;
    private float badPlanetChance = 0.3f;

    private void Start()
    {
        GenerateCaller(GetComponent<Chunk>().getTop() - GetComponent<Chunk>().height, transform);
        difficulty = GameManager.Instance.difficulty;
    }

    public async void GenerateCaller(float baseY, Transform parent)
    {
        await Task.Delay(200);
        StartCoroutine(GenerateChunkCoroutine(baseY, parent));
    }

    public IEnumerator GenerateChunkCoroutine(float baseY, Transform parent)
    {
        Random.InitState(System.Environment.TickCount ^ GetInstanceID());

        float originX = -(usableCols * cellSize) / 2f;
        Vector2 origin = new Vector2(originX, baseY);
        var grid = new ChunkGrid(origin, usableCols, Mathf.CeilToInt(chunkHeight / cellSize), cellSize);

        // Build spawner list based on difficulty
        var spawners = BuildSpawnerList(grid, parent);
        Shuffle(spawners);

        foreach (var spawner in spawners)
        {
            spawner();
            yield return new WaitForSeconds(0.2f);
        }

        // Spawn laser guns (difficulty-gated)
        if (difficulty >= laserGunThreshold)
            SpawnLaserGuns(baseY, parent);

        // Spawn missiles/rockets (difficulty-gated)
        if (difficulty >= rocketLauncherThreshold)
            SpawnSpread(grid, missilePrefab, Random.Range(1, 6), 1, 1, parent);

        yield return null;
    }

    /// <summary>
    /// Builds the list of spawners based on current difficulty level.
    /// </summary>
    private List<System.Action> BuildSpawnerList(ChunkGrid grid, Transform parent)
    {
        var spawners = new List<System.Action>();

        // Planets always spawn (but can be bad planets at max difficulty)
        spawners.Add(() => SpawnPlanetsSpread(grid, parent));

        // Spikes always spawn (basic obstacle)
        spawners.Add(() => SpawnSpread(grid, spikePrefab,
            Mathf.RoundToInt(Mathf.Lerp(5, 10, difficulty)), 2, 2, parent));

        // Black holes: difficulty >= 0.1
        if (difficulty >= blackHoleThreshold)
        {
            int blackHoleCount = difficulty >= 0.7f ? Random.Range(1, 3) : Random.Range(1, 3);
            spawners.Add(() => SpawnSpread(grid, blackHolePrefab, blackHoleCount, 5, 5, parent));
        }

        // Alien ships: difficulty >= 0.2
        if (difficulty >= alienShipThreshold)
        {
            spawners.Add(() => SpawnAliensSpread(grid, parent));
        }

        // Beam emitters: difficulty >= 0.4
        if (difficulty >= beamEmitterThreshold)
        {
            int beamCount = difficulty >= 0.8f ? Random.Range(1, 3) : Random.Range(1, 3);
            spawners.Add(() => SpawnSpread(grid, beamEmitterPrefab, beamCount, 4, 4, parent));
        }

        return spawners;
    }

    private void SpawnPlanetsSpread(ChunkGrid grid, Transform parent)
    {
        int planetCount = Mathf.RoundToInt(Mathf.Lerp(8, 5, difficulty));
        float baseSize = Mathf.Lerp(2f, 1f, difficulty);
        var placedPositions = new List<Vector2>();

        for (int i = 0; i < planetCount; i++)
        {
            float scale = baseSize * (1f + Random.Range(-0.3f, 0.3f));
            float moveX = Random.Range(0f, 2f);
            float moveY = Random.Range(0f, 1f);

            var pObj = new PlanetPlacable(Vector2.zero, scale * 2, moveX, moveY);
            Vector2Int bestCell = FindBestSpreadCell(grid, pObj, placedPositions);

            if (grid.TryPlaceAt(pObj, bestCell, out Vector2 pos))
            {
                var planet = Instantiate(planetPrefab, pos, Quaternion.identity, parent);
                planet.transform.localScale = Vector3.one * scale;

                // Set bad planet at max difficulty
                SetBadPlanetIfNeeded(planet);

                placedPositions.Add(pos);
            }
        }
    }

    /// <summary>
    /// Sets planet as bad planet based on difficulty threshold and random chance.
    /// </summary>
    private void SetBadPlanetIfNeeded(GameObject planet)
    {
        if (difficulty >= badPlanetThreshold && Random.value < badPlanetChance)
        {
            var planetAttribute = planet.GetComponent<PlanetAttribute>();
            if (planetAttribute != null)
            {
                // Use reflection or a setter method to set the private field
                SetBadPlanetProperty(planetAttribute, true);
            }
        }
    }

    /// <summary>
    /// Helper method to set isBadPlanet property (you'll need to add a setter to PlanetAttribute).
    /// </summary>
    private void SetBadPlanetProperty(PlanetAttribute planetAttribute, bool value)
    {
        // Method 1: If you add a public setter to PlanetAttribute
        // planetAttribute.SetBadPlanet(value);

        // Method 2: Using reflection (temporary solution)
        var field = typeof(PlanetAttribute).GetField("isBadPlanet",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(planetAttribute, value);
        }
    }

    private void SpawnAliensSpread(ChunkGrid grid, Transform parent)
    {
        int alienCount = Random.Range(1, 4);
        var placedPositions = new List<Vector2>();

        for (int i = 0; i < alienCount; i++)
        {
            var aObj = new AlienShipPlacable(Vector2.zero, 2, 1,
                Random.Range(1f, 3f), Random.Range(0f, 2f));
            Vector2Int bestCell = FindBestSpreadCell(grid, aObj, placedPositions);

            if (grid.TryPlaceAt(aObj, bestCell, out Vector2 pos))
            {
                Instantiate(alienShipPrefab, pos, Quaternion.identity, parent);
                placedPositions.Add(pos);
            }
        }
    }

    private void SpawnSpread(ChunkGrid grid, GameObject prefab, int count, int hSpan, int vSpan,
        Transform parent, Vector2Int? clampRangeX = null)
    {
        if (count <= 0) return;
        var placedPositions = new List<Vector2>();

        for (int i = 0; i < count; i++)
        {
            var sObj = new StaticPlacable(Vector2.zero, hSpan, vSpan);
            Vector2Int bestCell = FindBestSpreadCell(grid, sObj, placedPositions, clampRangeX);

            if (grid.TryPlaceAt(sObj, bestCell, out Vector2 pos, clampRangeX))
            {
                Instantiate(prefab, pos, Quaternion.identity, parent);
                placedPositions.Add(pos);
            }
        }
    }

    private Vector2Int FindBestSpreadCell(ChunkGrid grid, PlacableObject obj,
        List<Vector2> existingPositions, Vector2Int? rangeClampX = null)
    {
        var candidateCells = new List<Vector2Int>(grid.AvailableCells);
        ChunkGrid.ShuffleUtil.ShuffleInPlace(candidateCells);

        // First placement: pick one valid cell at random
        if (existingPositions.Count == 0)
        {
            var validCells = new List<Vector2Int>();
            foreach (var cell in candidateCells)
                if (grid.CanPlaceAt(obj, cell, rangeClampX))
                    validCells.Add(cell);

            if (validCells.Count > 0)
                return validCells[Random.Range(0, validCells.Count)];

            return Vector2Int.zero;
        }

        // Subsequent placements: keep spread heuristic
        float bestScore = -1f;
        Vector2Int bestCell = Vector2Int.zero;

        foreach (var cell in candidateCells)
        {
            if (!grid.CanPlaceAt(obj, cell, rangeClampX))
                continue;

            Vector2 worldPos = grid.CellToWorld(cell);
            float minDist = float.MaxValue;
            foreach (var placed in existingPositions)
                minDist = Mathf.Min(minDist, Vector2.Distance(worldPos, placed));

            float adjustedScore = minDist + Random.Range(0f, 0.5f);
            if (adjustedScore > bestScore)
            {
                bestScore = adjustedScore;
                bestCell = cell;
            }
        }
        return bestCell;
    }

    private void SpawnLaserGuns(float baseY, Transform parent)
    {
        int count = Random.Range(1, 3);
        for (int i = 0; i < count; i++)
        {
            float yPos = baseY + Random.Range(0f, chunkHeight);
            bool isLeft = Random.value < 0.5f;
            float xPos = isLeft ? -chunkWidth / 2f : chunkWidth / 2f;
            Vector2 pos = new Vector2(xPos, yPos);

            var laserGun = Instantiate(laserGunPrefab, pos, Quaternion.identity, parent);
            var gunScript = laserGun.GetComponent<LaserGunHandler>();
            if (gunScript != null) gunScript.isLeftGun = isLeft;
        }
    }

    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        for (int i = 0; i < n; i++)
        {
            int r = Random.Range(i, n);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}
