using Core;
using Environment;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public ProceduralSettings settings;
    public GameObject ball;

    private readonly List<GameObject> activeObjects = new();
    private readonly List<Vector2> planetPositions = new();
    private readonly List<Vector2> specialObjectPositions = new();
    private readonly List<Vector2> spikePositions = new();
    private readonly List<Vector2> zonePositions = new();

    private float leftBound;
    private float rightBound;
    private float nextStageY;
    private float nextTwinGateY;
    private float currentDifficulty;
    private int lastDistanceBannerHeight;

    private void Start()
    {
        CreateFixedStartPlanets();
        nextStageY = settings.stageHeight;
        nextTwinGateY = settings.stageHeight * 2; // starting buffer
    }

    private void Update()
    {
        if (ball.transform.position.y > nextStageY - settings.triggerOffset)
        {
            GenerateNewStage();
            nextStageY += settings.stageHeight;
            CleanupOldObjects();
        }
    }

   

    private void CreateFixedStartPlanets()
    {
        float[] fixedHeights = { 5f, 10f, 15f, 20f, 25f, 30f, 35f, 40f, 45f, 50f };
        float[] fixedSizes = { 4.5f, 4f, 6f, 5f, 4.3f, 2.7f, 3.6f, 5.3f, 3.8f, 3f };
        float[] fixedXPositions =
        {
            leftBound + 3f, rightBound - 3f,
            leftBound + 2f, rightBound - 2f,
            leftBound + 3.5f, rightBound - 3.5f,
            leftBound + 2.5f, rightBound - 2.5f,
            (leftBound + rightBound) / 2, (leftBound + rightBound) / 2
        };

        for (int i = 0; i < 10; i++)
            CreatePlanet(new Vector2(fixedXPositions[i], fixedHeights[i]), fixedSizes[i]*0.3f);
    }

    private void GenerateNewStage()
    {
        planetPositions.Clear();
        spikePositions.Clear();
        currentDifficulty = Mathf.Clamp01((ball.transform.position.y - settings.safeStartHeight) / 1000f);

        if (nextStageY % 500 == 0)
        {
            GameObject banner = Instantiate(settings.distanceBannerPrefab, new Vector3(0, nextStageY, -10), Quaternion.identity);
            banner.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{nextStageY}m";
            lastDistanceBannerHeight = (int)banner.transform.position.y;
        }

        PlacePlanets();
        FillGapsBetweenPlanets();
      //  PlacePowerups();

        if (ball.transform.position.y > settings.zoneStartHeight &&
            Random.value < settings.zoneSpawnChance &&
            zonePositions.Count < settings.maxZonesPerStage * 3)
        {
            PlaceZones();
        }

        if (ball.transform.position.y > settings.stageHeight &&
            ball.transform.position.y + settings.stageHeight > nextTwinGateY)
        {
            TrySpawnTwinGate();
        }
    }

    #region Twin Gates
    private bool IsLaserGunNear(float y, float range)
    {
        foreach (GameObject obj in activeObjects)
        {
            if (obj == null) continue;
            if (obj.CompareTag("laser_gun") && Mathf.Abs(obj.transform.position.y - y) < range)
                return true;
        }
        return false;
    }

    private void TrySpawnTwinGate()
    {
        float spawnY = nextTwinGateY + Random.Range(100f, 130f);
        if (IsLaserGunNear(spawnY, 6f)) return;

        bool leftFirst = Random.value < 0.5f;
        GameObject twinGate = Instantiate(settings.twinGatePrefab, new Vector3(0, spawnY, 0), Quaternion.identity);
        activeObjects.Add(twinGate);

        Transform gateA = twinGate.transform.GetChild(0);
        Transform gateB = twinGate.transform.GetChild(1);

        float leftX = GameManager.Instance.LeftBarX - 0.8f;
        float rightX = GameManager.Instance.RightBarX + 0.8f;

        if (leftFirst)
        {
            gateA.position = new Vector3(leftX, spawnY, 0f);
            gateB.position = new Vector3(rightX, spawnY + 6f, 0f);
        }
        else
        {
            gateA.position = new Vector3(rightX, spawnY, 0f);
            gateB.position = new Vector3(leftX, spawnY + 6f, 0f);
        }

        nextTwinGateY += Random.Range(200f, 300f);
    }
    #endregion

    #region Planets
    private void CreatePlanet(Vector2 pos, float size)
    {
        GameObject planet = Instantiate(settings.planetPrefab, pos, Quaternion.identity);
        planet.transform.localScale = Vector3.one * size;
        activeObjects.Add(planet);
    }

    private void PlacePlanets()
    {
        float currentY = nextStageY;
        float stageTop = nextStageY + settings.stageHeight;
        int maxPlanets = Mathf.FloorToInt(settings.stageHeight * settings.maxPlanetDensity);

        for (int i = 0; i < maxPlanets && currentY < stageTop; i++)
        {
            bool isBigPlanet = Random.value < settings.bigPlanetChance;
            float sizeBase = isBigPlanet
                ? settings.initialPlanetSize * settings.bigPlanetSizeMultiplier
                : Mathf.Lerp(settings.initialPlanetSize, settings.minPlanetSize,
                    settings.sizeReductionCurve.Evaluate(currentDifficulty));

            float size = sizeBase * (1 + Random.Range(-settings.planetSizeVariance, settings.planetSizeVariance * 0.8f));
            float distance = Random.Range(settings.minPlanetDistance, settings.maxPlanetDistance);

            Vector2 position = GetSafePlanetPosition(currentY, size, distance);

            if (position != Vector2.zero)
            {
                CreatePlanet(position, size);
                planetPositions.Add(position);
                currentY = position.y + distance;
            }
            else
            {
                currentY += distance * 0.5f;
            }
        }
    }

    private Vector2 GetSafePlanetPosition(float yPos, float size, float minDistance)
    {
        for (int attempts = 0; attempts < 100; attempts++)
        {
            float xPos = Random.value < 0.7f
                ? (leftBound + rightBound) / 2 + Random.Range(-1f, 1f) * (rightBound - leftBound) * 0.3f
                : Random.Range(leftBound + size + settings.minEdgeDistance, rightBound - size - settings.minEdgeDistance);

            Vector2 position = new(xPos, yPos);
            if (IsPositionSafe(position, size + minDistance * 0.7f))
                return position;
        }
        return Vector2.zero;
    }
    #endregion

    #region Zones
    private void PlaceZones()
    {
        int zonesPlaced = 0;
        int attempts = 0;

        while (zonesPlaced < settings.maxZonesPerStage && attempts < settings.zonePlacementAttempts)
        {
            attempts++;
            float yPos = nextStageY + Random.Range(20f, settings.stageHeight - 20f);

            if (!IsZonePositionValid(new Vector2(0, yPos))) continue;

            float blueX = Random.Range(leftBound + 2f, rightBound - 2f);
            Vector2 bluePos = new(blueX, yPos + Random.Range(5, 10));
            Vector2 redPos = new(
                (blueX < (leftBound + rightBound) / 2)
                    ? Random.Range((leftBound + rightBound) / 2 + settings.minHorizontalZoneDistance, rightBound - 2f)
                    : Random.Range(leftBound + 2f, (leftBound + rightBound) / 2 - settings.minHorizontalZoneDistance),
                yPos + Random.Range(3, 7)
            );

            if (IsZonePositionValid(bluePos) && IsZonePositionValid(redPos))
            {
                CreateBlueZone(bluePos, Random.Range(settings.minZoneSize, settings.maxZoneSize));
                CreateRedZone(redPos, Random.Range(settings.minZoneSize, settings.maxZoneSize));

                zonePositions.Add(bluePos);
                zonePositions.Add(redPos);
                zonesPlaced++;
            }
        }
    }

    private bool IsZonePositionValid(Vector2 pos)
    {
        foreach (Vector2 zonePos in zonePositions)
            if (Vector2.Distance(pos, zonePos) < settings.minVerticalZoneDistance)
                return false;

        foreach (Vector2 specialPos in specialObjectPositions)
            if (Vector2.Distance(pos, specialPos) < settings.minSpecialObjectDistance * 0.7f)
                return false;

        return true;
    }

    private void CreateBlueZone(Vector2 pos, float size)
    {
        GameObject blueZone = Instantiate(settings.safeZonePrefab, pos, Quaternion.identity);
        blueZone.transform.localScale = Vector3.one * size;
        activeObjects.Add(blueZone);
    }

    private void CreateRedZone(Vector2 pos, float size)
    {
        GameObject redZone = Instantiate(settings.dangerZonePrefab, pos, Quaternion.identity);
        redZone.transform.localScale = Vector3.one * size;
        activeObjects.Add(redZone);
    }
    #endregion

    #region Gaps, Coins & Powerups
    private void FillGapsBetweenPlanets()
    {
        if (planetPositions.Count < 2) return;

        planetPositions.Sort((a, b) => a.y.CompareTo(b.y));

        for (int i = 0; i < planetPositions.Count - 1; i++)
        {
            Vector2 bottom = planetPositions[i];
            Vector2 top = planetPositions[i + 1];
            float gapHeight = top.y - bottom.y;

            if (gapHeight > settings.minPlanetDistance)
            {
                int objectsToPlace = Mathf.Clamp(Mathf.FloorToInt(gapHeight / 8f), 1, 3);
                for (int j = 1; j <= objectsToPlace; j++)
                    PlaceRandomObjectAtHeight(bottom.y + (gapHeight * j / (objectsToPlace + 1)));
            }
        }
    }

    private void PlaceRandomObjectAtHeight(float yPos)
    {
        if (GetObjectDensity(yPos, 10f) > settings.maxPlanetDensity * 10f) return;

        List<float> xPositions = new();
        for (float x = leftBound + 2f; x <= rightBound - 2f; x += 2f)
            xPositions.Add(x);

        for (int i = xPositions.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i);
            (xPositions[i], xPositions[randomIndex]) = (xPositions[randomIndex], xPositions[i]);
        }

        foreach (float x in xPositions)
        {
            Vector2 position = new(x, yPos);
            if (!IsPositionSafe(position, settings.minObjectSpacing)) continue;

            if (ball.transform.position.y < settings.spikeStartHeight) continue;

            CreateSpike(position);
            spikePositions.Add(position);
            break;
        }
    }

    private void PlacePowerups()
    {
        for (int i = 0; i < settings.powerupsPerStage; i++)
        {
            if (Random.value <= 0.1f) continue;

            float yPos = nextStageY + Random.Range(10f, settings.stageHeight - 10f);
            float xPos = Random.Range(leftBound + 1f, rightBound - 1f);
            Vector2 pos = new(xPos, yPos);

            if (IsPositionSafe(pos, 2f))
                Instantiate(settings.shieldPrefab, pos, Quaternion.identity);
        }
    }

    private float GetObjectDensity(float yPos, float range)
    {
        float count = 0;
        foreach (var obj in activeObjects)
            if (obj != null && Mathf.Abs(obj.transform.position.y - yPos) < range)
                count++;
        return count / range;
    }
    #endregion

    private bool IsPositionSafe(Vector2 pos, float minDistance)
    {
        foreach (Vector2 p in planetPositions)
            if (Vector2.Distance(pos, p) < minDistance)
                return false;
        return true;
    }

    private void CreateSpike(Vector2 pos)
    {
        GameObject spike = Instantiate(settings.spikePrefab, pos, Quaternion.identity);
        activeObjects.Add(spike);
    }

    private void CleanupOldObjects()
    {
        activeObjects.RemoveAll(obj => obj == null);
    }
}
