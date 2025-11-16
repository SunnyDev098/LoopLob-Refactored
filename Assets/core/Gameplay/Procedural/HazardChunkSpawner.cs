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
        if (settings.alienShipPrefab1 != null && difficulty >= settings.alienShipThreshold)
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

        if (settings.coinPrefab != null)
        {
            spawners.Add(() => SpawnCoinTrailsSpread(grid, parent, difficulty, settings));
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
                var prefab = UnityEngine.Random.value < 0.7f ? s.alienShipPrefab1 : s.alienShipPrefab2;
                UnityEngine.Object.Instantiate(prefab, pos, Quaternion.identity, parent);

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




    private static void SpawnCoinTrailsSpread(ChunkGrid grid, Transform parent, float difficulty, ProceduralSettings s)
    {
        int trailCount = UnityEngine.Random.Range(s.coinTrailCountRange.x, s.coinTrailCountRange.y + 1);

        for (int t = 0; t < trailCount; t++)
        {
            // Select a random empty starting cell
            var candidateCells = new List<Vector2Int>(grid.AvailableCells);
            ChunkGrid.ShuffleUtil.ShuffleInPlace(candidateCells);

            Vector2Int startCell = candidateCells.Find(c => true); // first empty cell

            if (startCell == Vector2Int.zero)
                continue;

            // Generate path cells
            List<Vector2Int> pathCells = GenerateTrailPath(grid, startCell, s);

            if (pathCells.Count == 0)
                continue;

            // Mark cells as occupied by this trail (so no other hazards spawn there)
            foreach (var cell in pathCells)
            {
                Vector2 worldPos = grid.CellToWorld(cell);
                GameObject coin = UnityEngine.Object.Instantiate(s.coinPrefab, worldPos, Quaternion.identity, parent);
            }
        }
    }

    private static List<Vector2Int> GenerateTrailPath(ChunkGrid grid, Vector2Int startCell, ProceduralSettings s)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2 current = startCell;
        path.Add(startCell);

        int targetLength = UnityEngine.Random.Range(s.coinTrailMinLength, s.coinTrailMaxLength + 1);
        float halfStep = 0.5f;
        float noSpawnRadius = 0.8f * grid.CellSize;

        int curveState = 0;
        int curveHold = 0;
        int curveHoldMin = s.coinTrailCurveHoldMin;

        for (int i = 1; i < targetLength; i++)
        {
            if (curveState == 0)
            {
                curveState = UnityEngine.Random.Range(-1, 2);
                curveHold = 0;
            }
            else
            {
                curveHold++;
                if (curveHold >= curveHoldMin && UnityEngine.Random.value < s.coinTrailCurveExitChance)
                {
                    curveState = 0;
                    curveHold = 0;
                }
            }

            List<Vector2> directions = new List<Vector2>
        {
            new Vector2(curveState * halfStep, halfStep),
            new Vector2(curveState * 1f, 1f),
            new Vector2(0, 1f),
            new Vector2(0, halfStep)
        };

            bool placedNext = false;
            int lastY = path[path.Count - 1].y;

            foreach (var dir in directions)
            {
                Vector2 nextPos = current + dir;
                Vector2Int checkCell = new Vector2Int(Mathf.FloorToInt(nextPos.x), Mathf.FloorToInt(nextPos.y));

                if (checkCell.y <= lastY) continue;
                if (!grid.CanPlaceAt(new StaticPlacable(Vector2.zero, 1, 1), checkCell)) continue;

                Vector3 worldPos = grid.CellToWorld(checkCell);
                if (IsNearOtherObject(worldPos, noSpawnRadius, grid)) continue;

                path.Add(checkCell);
                current = nextPos;
                placedNext = true;
                break;
            }

            if (!placedNext) break;
        }

        return path;
    }

    private static bool IsNearOtherObject(Vector3 pos, float radius, ChunkGrid grid)
    {
        foreach (var occupied in grid.GetOccupiedWorldPositions())
        {
            if (Vector3.Distance(pos, occupied) < radius)
                return true;
        }
        return false;
    }








}
