using Environment;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Environment
{
    public static class HazardSpawner
    {
        public static void PlaceHazards(
            List<Vector2> planetPositions,
            List<GameObject> activeObjects,
            ProceduralSettings settings,
            float difficulty)
        {
            foreach (Vector2 planetPos in planetPositions)
            {
                // Decide if we put hazards in gaps between planets
                if (Random.value < settings.spikeProbability && Random.value < difficulty)
                {
                    TryPlaceSpikeNear(planetPos, activeObjects, settings);
                }

                if (Random.value < settings.blackHoleProbability && Random.value < difficulty)
                {
                    TryPlaceBlackHoleNear(planetPos, activeObjects, settings);
                }

                if (Random.value < settings.badPlanetProbability && Random.value < difficulty)
                {
                    TryPlaceBadPlanetNear(planetPos, activeObjects, settings);
                }
            }
        }

        private static void TryPlaceSpikeNear(Vector2 anchorPos, List<GameObject> activeObjects, ProceduralSettings s)
        {
            Vector2 pos = anchorPos + Random.insideUnitCircle * 2f;
            if (IsSafe(activeObjects, pos, s.minObjectSpacing))
            {
                GameObject spike = Object.Instantiate(s.spikePrefab, pos, Quaternion.identity);
                activeObjects.Add(spike);
            }
        }

        private static void TryPlaceBlackHoleNear(Vector2 anchorPos, List<GameObject> activeObjects, ProceduralSettings s)
        {
            Vector2 pos = anchorPos + Vector2.up * Random.Range(3f, 6f);
            if (IsSafe(activeObjects, pos, s.minObjectSpacing))
            {
                GameObject bh = Object.Instantiate(s.blackHolePrefab, pos, Quaternion.identity);
                activeObjects.Add(bh);
            }
        }

        private static void TryPlaceBadPlanetNear(Vector2 anchorPos, List<GameObject> activeObjects, ProceduralSettings s)
        {
            Vector2 pos = anchorPos + Vector2.up * Random.Range(4f, 7f);
            if (IsSafe(activeObjects, pos, s.minObjectSpacing))
            {
                GameObject badPlanet = Object.Instantiate(s.badPlanetPrefab, pos, Quaternion.identity);
                activeObjects.Add(badPlanet);
            }
        }

        private static bool IsSafe(List<GameObject> activeObjects, Vector2 pos, float minSpacing)
        {
            foreach (var obj in activeObjects)
            {
                if (obj == null) continue;
                if (Vector2.Distance(obj.transform.position, pos) < minSpacing)
                    return false;
            }
            return true;
        }
    }
}
