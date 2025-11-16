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

    [Header("Delay & Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    private AudioSource audioSource;
    private float enterTime;







    // rotation and alpha params

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 90f; // Degrees per second

    [Header("Alpha Fade Settings")]
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0.3f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;
    [SerializeField] private float fadeTime = 2f; // Time for full fade cycle

    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private float fadeTimer;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // --- Alpha Oscillation ---
        fadeTimer += Time.deltaTime;
        float t = Mathf.PingPong(fadeTimer / fadeTime, 1f);
        float newAlpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        // Apply alpha directly without creating new Color instances each frame
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
        // Delay before starting attraction
       

        Vector2 toCenter = (Vector2)transform.position - (Vector2)ball.transform.position;
        float distance = toCenter.magnitude;

        if (distance > maxAttractionDistance)
            return;

        float normalizedDistance = 1 - Mathf.Clamp01(
            (distance - minAttractionDistance) /
            (maxAttractionDistance - minAttractionDistance)
        );

        float curveMultiplier = attractionCurve.Evaluate(normalizedDistance);
        float attractionFactor = curveMultiplier  * attractionStrength * Time.deltaTime;

        Vector2 centerDirection = toCenter.normalized;
        Vector2 currentDirection = ball.GetMoveDirection();

        ball.SetMoveDirection(Vector2.Lerp(
            currentDirection.normalized,
            centerDirection,
            attractionFactor
        ).normalized * currentDirection.magnitude);
    }

    
}
