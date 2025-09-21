using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
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
    [SerializeField] private float attractionDelay = 0.5f;
    [SerializeField] private bool showDebugGizmos = true;
    private AudioSource audioSource;
    private float enterTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (audioSource != null)
            audioSource.volume = GameManager.Instance.SfxVolume * 3f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BallController>() != null)
            enterTime = Time.time;
    }

    public void OnAttracted(BallController ball)
    {
        // Delay before starting attraction
        if (Time.time < enterTime + attractionDelay)
            return;

        Vector2 toCenter = (Vector2)transform.position - (Vector2)ball.transform.position;
        float distance = toCenter.magnitude;

        if (distance > maxAttractionDistance)
            return;

        float normalizedDistance = 1 - Mathf.Clamp01(
            (distance - minAttractionDistance) /
            (maxAttractionDistance - minAttractionDistance)
        );

        float curveMultiplier = attractionCurve.Evaluate(normalizedDistance);
        float attractionFactor = curveMultiplier * 0.7f * attractionStrength * Time.deltaTime;

        Vector2 centerDirection = toCenter.normalized;
        Vector2 currentDirection = ball.GetMoveDirection();

        ball.SetMoveDirection(Vector2.Lerp(
            currentDirection.normalized,
            centerDirection,
            attractionFactor
        ).normalized * currentDirection.magnitude);
    }

    
}
