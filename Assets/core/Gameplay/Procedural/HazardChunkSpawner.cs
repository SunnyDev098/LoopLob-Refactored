using System;
using System.Collections.Generic;
using UnityEngine;
using Environment;

public static class HazardChunkSpawner
{
    public static List<Action> BuildSpawnerList(ChunkGrid grid, Transform parent, float difficulty, ProceduralSettings settings)
    {
        var spawners = new List<Action>();

        // Planets
        if (settings.planetPrefab != null)
            spawners.Add(() => SpawnPlanetsSpread(grid, parent, difficulty, settings));

        // Spikes
        if (settings.spikePrefab != null && difficulty >= settings.spikeThreshold)
        {
            int spikeCount = Mathf.RoundToInt(Mathf.Lerp(
                settings.spikeCountRange.x,
                settings.spikeCountRange.y,
                difficulty));
            spawners.Add(() => SpawnSpread(grid, settings.spikePrefab, spikeCount,
                settings.spikeHSpan, settings.spikeVSpan, parent));
        }

        // Black Holes
        if (settings.blackHolePrefab != null && difficulty >= settings.blackHoleThreshold)
        {
            int blackHoleCount = Mathf.RoundToInt(Mathf.Lerp(
                settings.blackHoleCountRange.x,
                settings.blackHoleCountRange.y,
                difficulty));
            spawners.Add(() => SpawnSpread(grid, settings.blackHolePrefab, blackHoleCount,
                settings.blackHoleHSpan, settings.blackHoleVSpan, parent));
        }

        // Alien Ships
        if (settings.alienShipPrefab != null && difficulty >= settings.alienShipThreshold)
        {
            spawners.Add(() => SpawnAliensSpread(grid, parent, difficulty, settings));
        }

        // Beam Emitters
        if (settings.beamEmitterPrefab != null && difficulty >= settings.beamEmitterThreshold)
        {
            int beamCount = Mathf.RoundToInt(Mathf.Lerp(
                settings.beamEmitterCountRange.x,
                settings.beamEmitterCountRange.y,
                difficulty));
            spawners.Add(() => SpawnSpread(grid, settings.beamEmitterPrefab, beamCount,
                settings.beamEmitterHSpan, settings.beamEmitterVSpan, parent));
        }

        // Laser Guns
        if (settings.laserGunPrefab != null && difficulty >= settings.laserGunThreshold)
        {
            int gunCount = UnityEngine.Random.Range(settings.laserGunCountRange.x, settings.laserGunCountRange.y + 1);
            spawners.Add(() => SpawnSpread(grid, settings.laserGunPrefab, gunCount,
             0, 0, parent));

        }

        // Missiles / Rockets
        if (settings.missilePrefab != null && difficulty >= settings.rocketLauncherThreshold)
        {
            int missileCount = UnityEngine.Random.Range(settings.missileCountRange.x, settings.missileCountRange.y);
            spawners.Add(() => SpawnSpread(grid, settings.missilePrefab, missileCount,
                1, 1, parent));
        }

        return spawners;
    }

    private static void SpawnPlanetsSpread(ChunkGrid grid, Transform parent, float difficulty, ProceduralSettings s)
    {
        int planetCount = Mathf.RoundToInt(Mathf.Lerp(s.planetCountRange.x, s.planetCountRange.y, difficulty));
        float baseSize = Mathf.Lerp(s.maxPlanetSize, s.minPlanetSize, difficulty);
        var placedPositions = new List<Vector2>();

        for (int i = 0; i < planetCount; i++)
        {
            float scale = baseSize * (1f + UnityEngine.Random.Range(-0.3f, 0.3f));
            var pObj = new PlanetPlacable(Vector2.zero, scale * 2, 0, 0);
            Vector2Int bestCell = FindBestSpreadCell(grid, pObj, placedPositions);

            if (grid.TryPlaceAt(pObj, bestCell, out Vector2 pos))
            {
                var planet = UnityEngine.Object.Instantiate(s.planetPrefab, pos, Quaternion.identity, parent);
                planet.transform.localScale = Vector3.one * scale;
                SetBadPlanetIfNeeded(planet, difficulty, s);
                placedPositions.Add(pos);
            }
        }
    }

    private static void SetBadPlanetIfNeeded(GameObject planet, float difficulty, ProceduralSettings s)
    {
        if (difficulty >= s.badPlanetThreshold && UnityEngine.Random.value < s.badPlanetChance)
        {
            var attr = planet.GetComponent<PlanetAttribute>();
            if (attr != null)
            {
                var field = typeof(PlanetAttribute).GetField("isBadPlanet",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null) field.SetValue(attr, true);
            }
        }
    }

    private static void SpawnAliensSpread(ChunkGrid grid, Transform parent, float difficulty, ProceduralSettings s)
    {
        int alienCount = UnityEngine.Random.Range(s.alienCountRange.x, s.alienCountRange.y);
        var placedPositions = new List<Vector2>();

        for (int i = 0; i < alienCount; i++)
        {
            var aObj = new AlienShipPlacable(Vector2.zero, 2, 1, UnityEngine.Random.Range(1f, 3f), UnityEngine.Random.Range(0f, 2f));
            Vector2Int bestCell = FindBestSpreadCell(grid, aObj, placedPositions);

            if (grid.TryPlaceAt(aObj, bestCell, out Vector2 pos))
            {
                UnityEngine.Object.Instantiate(s.alienShipPrefab, pos, Quaternion.identity, parent);
                placedPositions.Add(pos);
            }
        }
    }

    




    private static void SpawnSpread(ChunkGrid grid, GameObject prefab, int count, int hSpan, int vSpan, Transform parent)
    {
        if (count <= 0 || prefab == null) return;
        var placedPositions = new List<Vector2>();

        for (int i = 0; i < count; i++)
        {
            var sObj = new StaticPlacable(Vector2.zero, hSpan, vSpan);
            Vector2Int bestCell = FindBestSpreadCell(grid, sObj, placedPositions);

            if (grid.TryPlaceAt(sObj, bestCell, out Vector2 pos))
            {
                UnityEngine.Object.Instantiate(prefab, pos, Quaternion.identity, parent);
                placedPositions.Add(pos);
            }
        }
    }

    private static Vector2Int FindBestSpreadCell(ChunkGrid grid, PlacableObject obj, List<Vector2> existingPositions)
    {
        var candidateCells = new List<Vector2Int>(grid.AvailableCells);
        ChunkGrid.ShuffleUtil.ShuffleInPlace(candidateCells);

        if (existingPositions.Count == 0)
        {
            foreach (var cell in candidateCells)
                if (grid.CanPlaceAt(obj, cell))
                    return cell;
            return Vector2Int.zero;
        }

        float bestScore = -1f;
        Vector2Int bestCell = Vector2Int.zero;

        foreach (var cell in candidateCells)
        {
            if (!grid.CanPlaceAt(obj, cell))
                continue;

            Vector2 worldPos = grid.CellToWorld(cell);
            float minDist = float.MaxValue;
            foreach (var placed in existingPositions)
                minDist = Mathf.Min(minDist, Vector2.Distance(worldPos, placed));

            float adjustedScore = minDist + UnityEngine.Random.Range(0f, 0.5f);
            if (adjustedScore > bestScore)
            {
                bestScore = adjustedScore;
                bestCell = cell;
            }
        }
        return bestCell;
    }
}
