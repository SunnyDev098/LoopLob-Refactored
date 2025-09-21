using Core;
using Gameplay.Player;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class DirectionCurveTrigger : MonoBehaviour
{
    #region Serialized Fields
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

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    #endregion

    #region Private Fields
    private float enterTime;
    private AudioSource audioSource;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void SetInitialVolume()
    {
        if (audioSource != null)
            audioSource.volume *= GameManager.Instance.SfxVolume * 3f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("ball")) return;
        enterTime = Time.time;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Check tag first to avoid component lookups
        if (!other.CompareTag("ball")) return;

        // Optional delay before attraction starts
        if (enterTime + 0.5f < Time.time) return;

        var ball = other.GetComponent<BallController>();
        if (ball == null) return;

        ApplyAttraction(ball, other.transform);
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxAttractionDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, minAttractionDistance);

        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;
        for (float r = minAttractionDistance; r <= maxAttractionDistance; r += 0.5f)
        {
            float normalizedDist = 1 - Mathf.Clamp01(
                (r - minAttractionDistance) /
                (maxAttractionDistance - minAttractionDistance)
            );
            float strength = attractionCurve.Evaluate(normalizedDist) * attractionStrength;
            Vector3 pos = transform.position + Vector3.right * r;
            Gizmos.DrawLine(pos, pos + Vector3.up * strength * 0.1f);
        }
    }
    #endregion

    #region Private Methods
    private void ApplyAttraction(BallController ball, Transform ballTransform)
    {
        Vector2 toCenter = (Vector2)transform.position - (Vector2)ballTransform.position;
        float distance = toCenter.magnitude;

        if (distance > maxAttractionDistance) return;

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
    #endregion
}
