using UnityEngine;

namespace Environment
{
    [CreateAssetMenu(fileName = "ProceduralSettings", menuName = "Game/Procedural Settings")]
    public class ProceduralSettings : ScriptableObject
    {
        [Header("Prefabs")]
        public GameObject planetPrefab;
        public GameObject spikePrefab;
        public GameObject blackHolePrefab;
        public GameObject alienShipPrefab;
        public GameObject beamEmitterPrefab;
        public GameObject laserGunPrefab;
        public GameObject missilePrefab;
        public GameObject badPlanetPrefab;

        [Header("Difficulty Thresholds (0–1)")]
        [Range(0f, 1f)] public float spikeThreshold = 0.0f; // spikes spawn after this difficulty
        [Range(0f, 1f)] public float blackHoleThreshold = 0.06f;
        [Range(0f, 1f)] public float alienShipThreshold = 0.1f;
        [Range(0f, 1f)] public float beamEmitterThreshold = 0.2f;
        [Range(0f, 1f)] public float laserGunThreshold = 0.4f;
        [Range(0f, 1f)] public float rocketLauncherThreshold = 0.5f;
        [Range(0f, 1f)] public float badPlanetThreshold = 0.7f;

        [Header("Bad Planet Settings")]
        [Range(0f, 1f)] public float badPlanetChance = 0.3f;

        [Header("Spawn Counts Per Chunk")]
        public Vector2Int spikeCountRange = new Vector2Int(5, 10);
        public Vector2Int blackHoleCountRange = new Vector2Int(1, 3);
        public Vector2Int beamEmitterCountRange = new Vector2Int(1, 3);
        public Vector2Int planetCountRange = new Vector2Int(5, 8);
        public Vector2Int alienCountRange = new Vector2Int(1, 4);
        public Vector2Int missileCountRange = new Vector2Int(0, 2);

        [Header("Laser Gun Settings")]
        public Vector2Int laserGunCountRange = new Vector2Int(1, 3); // min/max guns per chunk
        public int laserGunHSpan = 3; // horizontal grid span in cells
        public int laserGunVSpan = 3; // vertical grid span in cells
        public float laserGunSafeEdgeMargin = 0.5f; // min distance from chunk edge

        [Header("Object Placement Settings")]
        public int spikeHSpan = 3;
        public int spikeVSpan = 3;
        public int blackHoleHSpan = 5;
        public int blackHoleVSpan = 5;
        public int beamEmitterHSpan = 4;
        public int beamEmitterVSpan = 4;
        public float minObjectSpacing = 2.5f;
        public float hazardSafeEdgeMargin = 0.5f;

        [Header("Planet Size Settings")]
        public float minPlanetSize = 1f;
        public float maxPlanetSize = 2f;
        public AnimationCurve planetSizeCurve = AnimationCurve.Linear(0f, 2f, 1f, 1f);
    }
}
