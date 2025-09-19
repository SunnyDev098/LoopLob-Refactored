using UnityEngine;

namespace Environment
{
    [CreateAssetMenu(fileName = "ProceduralSettings", menuName = "Game/Procedural Settings")]
    public class ProceduralSettings : ScriptableObject
    {
        [Header("References")]
        public GameObject spriteCarrier;

        [Header("Prefabs")]
        public GameObject planetPrefab;
        public GameObject triangleSpikePrefab;
        public GameObject laserGunPrefab;     // twin gate + laser
        public GameObject distanceBannerPrefab;
        public GameObject badPlanetPrefab;
        public GameObject blackHolePrefab;
        public GameObject spikePrefab;
        public GameObject magnetPrefab;
        public GameObject batteryPrefab;
        public GameObject shieldPrefab;
        public GameObject blueZonePrefab;
        public GameObject redZonePrefab;
        public GameObject coinPrefab;

        [Header("Screen Margins")]
        [Range(0.7f, 0.95f)]
        public float horizontalMargin = 0.85f;

        [Header("Planet Settings")]
        public float minPlanetDistance = 4f;
        public float maxPlanetDistance = 7f;
        public float initialPlanetSize = 2.5f;
        public float minPlanetSize = 1.0f;
        [Range(0f, 1f)] public float bigPlanetChance = 0.15f;
        public float bigPlanetSizeMultiplier = 2f;
        public float planetSizeVariance = 0.3f;
        public float minEdgeDistance = 1.5f;
        public AnimationCurve sizeReductionCurve;
        public AnimationCurve distanceIncreaseCurve;

        [Header("Stage Settings")]
        public float stageHeight = 100f;
        public float triggerOffset = 30f;

        [Header("Twin Gate Settings")]
        public float twinGateStartHeight = 1000f;
        public Vector2 twinGateSpawnInterval = new Vector2(200f, 300f);

        [Header("Obstacle Settings")]
        public int safeStartHeight = 50;
        public int blackHoleStartHeight = 150;
        public int spikeStartHeight = 100;
        public int badPlanetStartHeight = 200;
        public float minSpecialObjectDistance = 15f;

        [Header("Density Controls")]
        [Range(0f, 1f)] public float maxPlanetDensity = 0.3f;
        [Range(0f, 1f)] public float maxSpikeDensity = 0.4f;
        public float minObjectSpacing = 2.5f;

        [Header("Item Distribution")]
        [Range(0f, 1f)] public float spikeProbability = 0.4f;
        [Range(0f, 1f)] public float blackHoleProbability = 0.2f;
        [Range(0f, 1f)] public float badPlanetProbability = 0.2f;
        public int powerupsPerStage = 3;

        [Header("Zone Settings")]
        public int zoneStartHeight = 200;
        [Range(0f, 1f)] public float zoneSpawnChance = 0.5f;
        public float minVerticalZoneDistance = 10f;
        public float minHorizontalZoneDistance = 5f;
        public int zonePlacementAttempts = 5;
        public int maxZonesPerStage = 2;
        [Range(0f, 1f)] public float zoneSizeVariance = 0.3f;
        public float minZoneSize = 1.5f;
        public float maxZoneSize = 3f;

        [Header("Coin Trail Settings")]
        [Range(0f, 1f)] public float coinTrailChance = 0.5f; // probability of generating planet-based trails
        public float coinSpacing = 1.0f;
        public float minCurveHeight = 1.0f;
        public float maxCurveHeight = 3.0f;
        public float obstacleAvoidOffset = 2.0f;
        public bool allowCurvedTrails = true;
        public float planetSafeOffset = 1.5f;
        [Range(0f, 1f)] public float freeCoinPathChance = 0.3f;

        [Header("Banner & Fade")]
        public float distanceBannerInterval = 200f;
        public float fadeDuration = 3f;



        [Header("Extra Prefabs")]
        public GameObject twinGatePrefab;    // old laser gun twin gate prefab
       // public GameObject dangerZonePrefab;
       // public GameObject safeZonePrefab;
    }
}
