using Gameplay.Player;
using UnityEngine;

/// <summary>
/// Holds data and simple behavior for a planet.
/// Responsible for calculating orbit radius from planet size.
/// </summary>
public class PlanetAttribute : MonoBehaviour
{
    [Header("Orbit Settings")]
    [Tooltip("Orbit visual GameObject (optional)")]
    [SerializeField] private GameObject activatedOrbit;
    [Tooltip("Orbit radius in world units, based on planet size")]
    public float orbitRadius { get; private set; }

    [Header("Movement Settings")]
    [Tooltip("Whether the planet moves horizontally back and forth")]
    public bool movingPlanet = false;
    [SerializeField] private float xRange = 3f;
    [SerializeField] private AnimationCurve verticalCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 startPosition;
    private float minXLimit;
    private float maxXLimit;
    private float moveTimer;

    private void Start()
    {
        // Set limits based on planet edges
        minXLimit = -xRange;
        maxXLimit = xRange;
        startPosition = transform.position;

        // Calculate orbit radius based on planet size
        orbitRadius = (transform.localScale.x * 1.5f) / 2f;

        // Hide orbit visual initially
        if (activatedOrbit != null)
            activatedOrbit.SetActive(false);
    }

    private void Update()
    {
        if (movingPlanet)
            HandleMovement();
    }

    /// <summary>
    /// Simple horizontal oscillation.
    /// </summary>
    private void HandleMovement()
    {
        moveTimer += Time.deltaTime;
        float t = Mathf.PingPong(moveTimer, 1f);
        float curved = verticalCurve.Evaluate(t);
        float offset = Mathf.Lerp(minXLimit, maxXLimit, curved);

        transform.position = startPosition + new Vector3(offset, 0f, 0f);
    }

    /// <summary>
    /// Activates the orbit visual with a short scale animation.
    /// </summary>
    public void ActivateOrbitVisual()
    {
        if (activatedOrbit == null) return;
        activatedOrbit.SetActive(true);
        StartCoroutine(ScaleOrbitRoutine(0.2f));
    }

    private System.Collections.IEnumerator ScaleOrbitRoutine(float duration)
    {
        Vector3 targetScale = activatedOrbit.transform.localScale;
        activatedOrbit.transform.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            activatedOrbit.transform.localScale =
                Vector3.Lerp(Vector3.zero, targetScale, progress);
            yield return null;
        }

        activatedOrbit.transform.localScale = targetScale;
    }

    /// <summary>
    /// Toggles the planet's movement direction (if moving).
    /// </summary>
    public void ToggleMovementDirection()
    {
        xRange = -xRange;
        minXLimit = -xRange;
        maxXLimit = xRange;
    }

}
