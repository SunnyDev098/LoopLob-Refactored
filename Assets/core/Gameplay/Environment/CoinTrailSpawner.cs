using Environment;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Environment
{
    public static class CoinTrailSpawner
    {
        public static void SpawnTrails(
            List<Vector2> planetPositions,
            List<GameObject> activeObjects,
            ProceduralSettings s)
        {
            if (planetPositions.Count < 2) return;

            planetPositions.Sort((a, b) => a.y.CompareTo(b.y));

            for (int i = 0; i < planetPositions.Count - 1; i++)
            {
                Vector2 from = planetPositions[i];
                Vector2 to = planetPositions[i + 1];

                if (Random.value > s.coinTrailChance) continue;

                bool curved = s.allowCurvedTrails && Random.value > s.freeCoinPathChance;
                List<Vector2> points = GeneratePath(from, to, curved, s);

                foreach (Vector2 p in points)
                {
                    if (!IsBlocked(p, activeObjects, s.planetSizeVariance))
                    {
                        GameObject coin = Object.Instantiate(s.coinPrefab /* your coin prefab here */, p, Quaternion.identity);
                        activeObjects.Add(coin);
                    }
                }
            }
        }

        private static List<Vector2> GeneratePath(Vector2 start, Vector2 end, bool curved, ProceduralSettings s)
        {
            List<Vector2> points = new();
            int resolution = 10;
            float distAcc = 0f;

            if (curved)
            {
                Vector2 control = (start + end) / 2 + Vector2.up * Random.Range(s.minPlanetDistance, s.maxPlanetDistance);
                for (int i = 0; i <= resolution; i++)
                {
                    float t = i / (float)resolution;
                    Vector2 a = Vector2.Lerp(start, control, t);
                    Vector2 b = Vector2.Lerp(control, end, t);
                    Vector2 point = Vector2.Lerp(a, b, t);
                    if (points.Count == 0 || Vector2.Distance(points[^1], point) >= s.coinSpacing)
                        points.Add(point);
                }
            }
            else
            {
                for (int i = 0; i <= resolution; i++)
                {
                    float t = i / (float)resolution;
                    Vector2 point = Vector2.Lerp(start, end, t);
                    if (points.Count == 0 || Vector2.Distance(points[^1], point) >= s.coinSpacing)
                        points.Add(point);
                }
            }

            return points;
        }

        private static bool IsBlocked(Vector2 pos, List<GameObject> activeObjects, float safeOffset)
        {
            foreach (var obj in activeObjects)
            {
                if (obj == null) continue;
                if (Vector2.Distance(pos, obj.transform.position) < safeOffset)
                    return true;
            }
            return false;
        }
    }
}
