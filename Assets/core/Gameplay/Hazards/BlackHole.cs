using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using System.Threading.Tasks;
using UnityEngine;

public class BlackHole : MonoBehaviour, IAttractor
{
    [Header("Attraction Settings")]
    [Tooltip("How strongly the direction curves toward center")]
    [SerializeField] private float attractionStrength = 5f;

    [Tooltip("Maximum distance for attraction effect")]
    [SerializeField] private float maxAttractionDistance = 5f;

    [Tooltip("Distance where attraction is strongest")]
    [SerializeField] private float minAttractionDistance = 0.5f;

    [Tooltip("Controls how quickly attraction increases as ball approaches center")]
    [SerializeField]
    private AnimationCurve attractionCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(1f, 1f)
    );

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 90f; // Degrees per second

    [Header("Alpha Fade Settings")]
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0.3f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;
    [SerializeField] private float fadeTime = 2f; // Time for full fade cycle

    [Header("Difficulty Scaling")]
    [Tooltip("Min and Max scale when Difficulty is 0")]
    [SerializeField] private Vector2 scaleRangeEasy = new Vector2(0.1f, 0.3f);

    [Tooltip("Min and Max scale when Difficulty is 1")]
    [SerializeField] private Vector2 scaleRangeHard = new Vector2(0.5f, 0.9f);

    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private float fadeTimer;
    private AudioSource audioSource;
    private float enterTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
    }

    private async void Start()
    {
        await Task.Delay(500);
        ApplyDifficultyScaling();
        CheckInitialOverlap();
    }

    private void ApplyDifficultyScaling()
    {
        // Get difficulty (ensure it is clamped between 0 and 1)
        float diff = Mathf.Clamp01(GameManager.Instance.difficulty);

        // 1. Calculate the dynamic MINIMUM limit based on difficulty
        // At diff 0 -> 0.3, At diff 1 -> 0.8
        float currentMinLimit = Mathf.Lerp(scaleRangeEasy.x, scaleRangeHard.x, diff);

        // 2. Calculate the dynamic MAXIMUM limit based on difficulty
        // At diff 0 -> 0.5, At diff 1 -> 1.4
        float currentMaxLimit = Mathf.Lerp(scaleRangeEasy.y, scaleRangeHard.y, diff);

        // 3. Pick a random size within this calculated window
        float finalScale = Random.Range(currentMinLimit, currentMaxLimit);

        transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // --- Alpha Oscillation ---
        fadeTimer += Time.deltaTime;
        float t = Mathf.PingPong(fadeTimer / fadeTime, 1f);
        float newAlpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        // Apply alpha directly
        baseColor.a = newAlpha;
        spriteRenderer.color = baseColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BallController>() != null)
            enterTime = Time.time;
    }

    public void OnAttracted(BallController ball)
    {
        Vector2 toCenter = (Vector2)transform.position - (Vector2)ball.transform.position;
        float distance = toCenter.magnitude;

        if (distance > maxAttractionDistance)
            return;

        float normalizedDistance = 1 - Mathf.Clamp01(
            (distance - minAttractionDistance) /
            (maxAttractionDistance - minAttractionDistance)
        );

        float curveMultiplier = attractionCurve.Evaluate(normalizedDistance);
        float attractionFactor = curveMultiplier * attractionStrength * Time.deltaTime;

        Vector2 centerDirection = toCenter.normalized;
        Vector2 currentDirection = ball.GetMoveDirection();

        ball.SetMoveDirection(Vector2.Lerp(
            currentDirection.normalized,
            centerDirection,
            attractionFactor
        ).normalized * currentDirection.magnitude);
    }

    private void CheckInitialOverlap()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (hit.enabled && hit.CompareTag("OuterOrbit"))
            {
                Destroy(gameObject);
                return;
            }
        }
    }

}
