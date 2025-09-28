using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlanetGenerator : MonoBehaviour
{
    [Header("Difficulty Settings")]
    [Range(0f, 1f)]
    public float difficulty;

    [Header("Chunk Bounds")]
    public float chunkHeight = 50f;
    public float chunkWidth = 10f;
    public float safeEdgeMargin = 0.5f;
    public float minObjectSpacing = 2.5f;
    public float minPlanetVerticalSpacing = 5f;

    [Header("Prefabs")]
    public GameObject planetPrefab;
    public GameObject spikePrefab;
    public GameObject blackHolePrefab;
    public GameObject laserGunPrefab;
    public GameObject alienShipPrefab;
    public GameObject missilePrefab;
    public GameObject beamEmitterPrefab;

    private float leftBound, rightBound;
    private List<Vector2> occupiedPositions = new List<Vector2>();

    private void Start()
    {
        GenerateCaller(GetComponent<Chunk>().getTop() - GetComponent<Chunk>().height, transform);
    }
    public async void GenerateCaller(float baseY, Transform parent)
    {

        await Task.Delay(200);
        StartCoroutine(GenerateChunkCoroutine(baseY, parent));
    }

    public IEnumerator GenerateChunkCoroutine(float baseY, Transform parent)
    {
        CalculateHorizontalBounds();
        occupiedPositions.Clear();

        yield return StartCoroutine(GeneratePlanetsCoroutine(baseY, parent));
        yield return StartCoroutine(GenerateHazardsInGapsCoroutine(baseY, parent));
    }

    private void CalculateHorizontalBounds()
    {
        leftBound = -chunkWidth / 2f + safeEdgeMargin;
        rightBound = chunkWidth / 2f - safeEdgeMargin;
    }

    private IEnumerator GeneratePlanetsCoroutine(float baseY, Transform parent)
    {
        int planetCount = Mathf.RoundToInt(Mathf.Lerp(7, 4, difficulty));
        float baseSize = Mathf.Lerp(1.5f, 1f, difficulty);

        float verticalStep = chunkHeight / planetCount;

        for (int i = 0; i < planetCount; i++)
        {
            float yPos = baseY + i * verticalStep + Random.Range(-1f, 1f);
            float xPos = Random.Range(leftBound, rightBound);

            Vector2 pos = new Vector2(xPos, yPos);
            if (!IsPositionSafe(pos, minObjectSpacing)) { i--; continue; }

            float size = baseSize * (1f + Random.Range(-0.3f, 0.3f));
            GameObject planet = Instantiate(planetPrefab, pos, Quaternion.identity, parent);
            planet.transform.localScale = Vector3.one * size;

            occupiedPositions.Add(pos);
            yield return null;
        }
    }

    private IEnumerator GenerateHazardsInGapsCoroutine(float baseY, Transform parent)
    {
        bool CanPlaceHazardHere(Vector2 pos) => IsPositionSafe(pos, minObjectSpacing);

        SpawnSpikes(baseY, parent, CanPlaceHazardHere);
        SpawnBlackHoles(baseY, parent, CanPlaceHazardHere);
        SpawnLaserGuns(baseY, parent); // has built-in fixed X logic
        SpawnBeamEmitters(baseY, parent, CanPlaceHazardHere);
        SpawnAlienShips(baseY, parent, CanPlaceHazardHere);
        SpawnMissiles(baseY, parent, CanPlaceHazardHere);

        yield return null;
    }

    // --- Hazard-specific methods ---
    private void SpawnSpikes(float baseY, Transform parent, System.Func<Vector2, bool> positionCheck)
    {
        int spikeCount = Mathf.RoundToInt(Mathf.Lerp(5, 10, difficulty));
        SpawnHazard(spikePrefab, spikeCount, baseY, parent, positionCheck);
    }

    private void SpawnBlackHoles(float baseY, Transform parent, System.Func<Vector2, bool> positionCheck)
    {
        if (difficulty >= 0.3f)
        {
            int count = Random.Range(1, difficulty >= 0.7f ? 3 : 2);
            SpawnHazard(blackHolePrefab, count, baseY, parent, positionCheck);
        }
    }

    private void SpawnLaserGuns(float baseY, Transform parent)
    {
        if (difficulty >= 0.5f)
        {
            int count = Random.Range(1, 3);
            for (int i = 0; i < count; i++)
            {
                float yPos = baseY + Random.Range(0f, chunkHeight);
                // Decide which side to place the laser
                bool isLeft = Random.value < 0.5f;
                float xPos = isLeft ? -5f : 5f;
                Vector2 pos = new Vector2(xPos, yPos);

                if (!IsPositionSafe(pos, minObjectSpacing)) { i--; continue; }

                GameObject laserGun = Instantiate(laserGunPrefab, pos, Quaternion.identity, parent);
                // Set the is_left property on the laser gun script
                var gunScript = laserGun.GetComponent<LaserGunHandler>();
                if (gunScript != null)
                    gunScript.isLeftGun = isLeft;

                occupiedPositions.Add(pos);
            }
        }
    }

    private void SpawnBeamEmitters(float baseY, Transform parent, System.Func<Vector2, bool> positionCheck)
    {
        if (difficulty >= 0.4f)
        {
            int count = Random.Range(1, difficulty >= 0.8f ? 3 : 2);
            SpawnHazard(beamEmitterPrefab, count, baseY, parent, positionCheck);
        }
    }

    private void SpawnAlienShips(float baseY, Transform parent, System.Func<Vector2, bool> positionCheck)
    {
        if (difficulty >= 0.7f)
        {
            int count = Random.Range(1, 4);
            SpawnHazard(alienShipPrefab, count, baseY, parent, positionCheck);
        }
    }

    private void SpawnMissiles(float baseY, Transform parent, System.Func<Vector2, bool> positionCheck)
    {
        if (difficulty >= 0.8f)
        {
            int count = Random.Range(1, 6);
            SpawnHazard(missilePrefab, count, baseY, parent, positionCheck);
        }
    }

    // --- General hazard placement ---
    private void SpawnHazard(GameObject prefab, int count, float baseY, Transform parent, System.Func<Vector2, bool> positionCheck)
    {
        int tries = 0;
        for (int i = 0; i < count && tries < count * 5; i++)
        {
            tries++;
            float yPos = baseY + Random.Range(0f, chunkHeight);
            float xPos = Random.Range(leftBound, rightBound);

            Vector2 pos = new Vector2(xPos, yPos);
            if (!positionCheck(pos)) { i--; continue; }

            Instantiate(prefab, pos, Quaternion.identity, parent);
            occupiedPositions.Add(pos);
        }
    }

    private bool IsPositionSafe(Vector2 pos, float spacing)
    {
        foreach (Vector2 existing in occupiedPositions)
        {
            if (Vector2.Distance(pos, existing) < spacing)
                return false;
        }
        return true;
    }
}
