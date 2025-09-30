using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Core chunk generator using PlacableObject system.
/// Controls planet and hazard placement.
/// </summary>
public class ChunkGenerator : MonoBehaviour
{
    [Header("Difficulty Settings")]
    [Range(0f, 1f)]
    public float difficulty;

    [Header("Chunk Bounds")]
    public float chunkHeight = 50f;
    public float chunkWidth = 10f;
    public float safeEdgeMargin = 0.5f;

    [Header("Prefabs")]
    public GameObject planetPrefab;
    public GameObject spikePrefab;
    public GameObject blackHolePrefab;
    public GameObject laserGunPrefab;
    public GameObject alienShipPrefab;
    public GameObject rocketLauncherPrefab;
    public GameObject beamEmitterPrefab;

    private float leftBound, rightBound;
    private List<PlacableObject> placedObjects = new List<PlacableObject>();

    private void Start()
    {
        GenerateCaller(GetComponent<Chunk>().getTop() - GetComponent<Chunk>().height, transform);
    }

    public async void GenerateCaller(float baseY, Transform parent)
    {
        await Task.Delay(200);
        StartCoroutine(GenerateChunkCoroutine(baseY, parent));
    }

    private IEnumerator GenerateChunkCoroutine(float baseY, Transform parent)
    {
        CalculateHorizontalBounds();
        placedObjects.Clear();

        yield return StartCoroutine(GeneratePlanetsCoroutine(baseY, parent));
        yield return StartCoroutine(GenerateHazardsCoroutine(baseY, parent));
    }

    private void CalculateHorizontalBounds()
    {
        leftBound = -chunkWidth / 2f + safeEdgeMargin;
        rightBound = chunkWidth / 2f - safeEdgeMargin;
    }

    // ------------------- PLANET GENERATION -------------------
    private IEnumerator GeneratePlanetsCoroutine(float baseY, Transform parent)
    {
        int planetCount = Mathf.RoundToInt(Mathf.Lerp(7, 4, difficulty));
        float baseSize = Mathf.Lerp(1.5f, 1f, difficulty);
        float verticalStep = chunkHeight / planetCount;
        int maxAttempts = planetCount * 5;

        for (int i = 0, attempts = 0; i < planetCount && attempts < maxAttempts; attempts++)
        {
            float yPos = baseY + i * verticalStep + Random.Range(-1f, 1f);
            float xPos = Random.Range(leftBound, rightBound);
            Vector2 pos = new Vector2(xPos, yPos);

            if (!IsPositionSafe(pos, 2f)) { continue; } // Planet safe radius

            float size = baseSize * (1f + Random.Range(-0.3f, 0.3f));
            var planetGO = Instantiate(planetPrefab, pos, Quaternion.identity, parent);
            planetGO.transform.localScale = Vector3.one * size;
            placedObjects.Add(new PlanetPlacable(planetGO.transform));

            i++;
            yield return null;
        }
    }

    // ------------------- HAZARD GENERATION -------------------
    private IEnumerator GenerateHazardsCoroutine(float baseY, Transform parent)
    {
        SpawnSpikes(baseY, parent);
        SpawnBlackHoles(baseY, parent);
        SpawnBeamEmitters(baseY, parent);
        SpawnAlienShips(baseY, parent);
        SpawnLaserGuns(baseY, parent);
        SpawnRocketLaunchers(baseY, parent);
        yield return null;
    }

    private void SpawnSpikes(float baseY, Transform parent)
    {
        int count = Mathf.RoundToInt(Mathf.Lerp(5, 10, difficulty));
        int maxAttempts = count * 5;

        for (int i = 0, attempts = 0; i < count && attempts < maxAttempts; attempts++)
        {
            Vector2 pos = GetRandomChunkPos(baseY);
            if (!IsPositionSafe(pos, 1f)) { continue; }

            var spikeGO = Instantiate(spikePrefab, pos, Quaternion.identity, parent);
            placedObjects.Add(new SpikePlacable(spikeGO.transform));
            i++;
        }
    }

    private void SpawnBlackHoles(float baseY, Transform parent)
    {
        if (difficulty < 0.3f) return;
        int count = Random.Range(1, difficulty >= 0.7f ? 3 : 2);
        int maxAttempts = count * 5;

        for (int i = 0, attempts = 0; i < count && attempts < maxAttempts; attempts++)
        {
            Vector2 pos = GetRandomChunkPos(baseY);
            if (!IsPositionSafe(pos, 3f)) { continue; }

            var bhGO = Instantiate(blackHolePrefab, pos, Quaternion.identity, parent);
            placedObjects.Add(new BlackHolePlacable(bhGO.transform));
            i++;
        }
    }

    private void SpawnBeamEmitters(float baseY, Transform parent)
    {
        if (difficulty < 0.4f) return;
        int count = Random.Range(1, difficulty >= 0.8f ? 3 : 2);
        int maxAttempts = count * 5;

        for (int i = 0, attempts = 0; i < count && attempts < maxAttempts; attempts++)
        {
            Vector2 pos = GetRandomChunkPos(baseY);
            if (!IsPositionSafe(pos, 2.5f)) { continue; }

            var beamGO = Instantiate(beamEmitterPrefab, pos, Quaternion.identity, parent);
            placedObjects.Add(new BeamEmitterPlacable(beamGO.transform));
            i++;
        }
    }

    private void SpawnAlienShips(float baseY, Transform parent)
    {
        if (difficulty < 0.7f) return;
        int count = Random.Range(1, 4);
        int maxAttempts = count * 5;

        for (int i = 0, attempts = 0; i < count && attempts < maxAttempts; attempts++)
        {
            Vector2 pos = GetRandomChunkPos(baseY);
            if (!IsPositionSafe(pos, 2f)) { continue; }

            var alienGO = Instantiate(alienShipPrefab, pos, Quaternion.identity, parent);
            placedObjects.Add(new AlienShipPlacable(alienGO.transform));
            i++;
        }
    }

    private void SpawnLaserGuns(float baseY, Transform parent)
    {
        if (difficulty < 0.5f) return;
        int count = Random.Range(1, 3);
        int maxAttempts = count * 5;

        for (int i = 0, attempts = 0; i < count && attempts < maxAttempts; attempts++)
        {
            float yPos = baseY + Random.Range(0, chunkHeight);
            bool isLeft = Random.value < 0.5f;
            float xPos = isLeft ? -5f : 5f;
            Vector2 pos = new Vector2(xPos, yPos);

            if (!IsPositionSafe(pos, 1.5f)) { continue; }

            var laserGO = Instantiate(laserGunPrefab, pos, Quaternion.identity, parent);
            var gunScript = laserGO.GetComponent<LaserGunHandler>();
            if (gunScript != null)
                gunScript.isLeftGun = isLeft;

            placedObjects.Add(new LaserGunPlacable(laserGO.transform));
            i++;
        }
    }

    private void SpawnRocketLaunchers(float baseY, Transform parent)
    {
        if (difficulty < 0.8f) return;
        int count = Random.Range(1, 6);
        int maxAttempts = count * 5;

        for (int i = 0, attempts = 0; i < count && attempts < maxAttempts; attempts++)
        {
            Vector2 pos = GetRandomChunkPos(baseY);
            if (!IsPositionSafe(pos, 2f)) { continue; }

            var rocketGO = Instantiate(rocketLauncherPrefab, pos, Quaternion.identity, parent);
            placedObjects.Add(new RocketLauncherPlacable(rocketGO.transform));
            i++;
        }
    }

    // ------------------- HELPER METHODS -------------------
    private Vector2 GetRandomChunkPos(float baseY)
    {
        float xPos = Random.Range(leftBound, rightBound);
        float yPos = baseY + Random.Range(0f, chunkHeight);
        return new Vector2(xPos, yPos);
    }

    private bool IsPositionSafe(Vector2 pos, float safeRadius)
    {
        var temp = new TempPlacable(pos, safeRadius);
        return temp.InSafeRadius(pos, placedObjects);
    }

    private class TempPlacable : PlacableObject
    {
        private Vector2 position;
        public TempPlacable(Vector2 pos, float sr) : base(sr, 0, 0)
        {
            position = pos;
        }
        public override Vector2 GetPosition() => position;
    }
}
