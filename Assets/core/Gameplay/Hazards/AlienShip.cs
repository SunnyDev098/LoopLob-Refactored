using UnityEngine;
using System.Collections;
using Gameplay.Interfaces;
using Gameplay.Player;
using Core;

namespace Gameplay.Player
{
    /// <summary>
    /// Alien enemy that wanders in a PingPong pattern and cycles sprites when hitting the ball.
    /// Damage application is routed through the IHitBall interface.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class AlienShip : MonoBehaviour, IHitBall
    {
        [Header("Sprites")]
        [Tooltip("Assign 3 sprites for the animation cycle.")]
        [SerializeField] private Sprite[] sprites = new Sprite[3];

        [Header("Animation Settings")]
        [SerializeField] private float spriteFrameDelay = 0.05f;

        [Header("Horizontal Movement")]
        [SerializeField] private float xRange = 3f;
        [SerializeField] private float xDuration = 2f;

        [Header("Vertical Movement")]
        [SerializeField] private float yRange = 2f;
        [SerializeField] private float yDuration = 2f;

        [Tooltip("S-curve for smoother motion. Defaults to EaseInOut if empty.")]
        [SerializeField] private AnimationCurve movementCurve = null;

        private SpriteRenderer spriteRenderer;
        private Vector3 startPosition;
        private float tX;
        private float tY;
        private WaitForSeconds frameDelay;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            frameDelay = new WaitForSeconds(spriteFrameDelay);
        }

        private void Start()
        {
            startPosition = transform.position;

            if (movementCurve == null || movementCurve.length == 0)
                movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        private void Update()
        {
            UpdateMovement();
        }

        /// <summary>
        /// Called by BallInteraction when the ball enters this object's trigger.
        /// </summary>
        public void OnHitBall(BallController ballController)
        {
            // Respect game state
            if (GameManager.Instance.NoDeath || GameManager.Instance.IsBallAttached())
                return;

            CycleSprites(ballController);
        }

        private void UpdateMovement()
        {
            // Horizontal motion
            tX += Time.deltaTime / Mathf.Max(0.0001f, xDuration);
            float xOffset = Mathf.Lerp(-xRange, xRange, movementCurve.Evaluate(Mathf.PingPong(tX, 1f)));

            // Vertical motion
            tY += Time.deltaTime / Mathf.Max(0.0001f, yDuration);
            float yOffset = Mathf.Lerp(-yRange, yRange, movementCurve.Evaluate(Mathf.PingPong(tY, 1f)));

            transform.position = startPosition + new Vector3(xOffset, yOffset, 0f);
        }

        private void CycleSprites(BallController ball)
        {
            StartCoroutine(CycleSpritesCoroutine(ball));
        }

        private IEnumerator CycleSpritesCoroutine(BallController ball)
        {
            if (sprites.Length < 3 || spriteRenderer == null)
                yield break;

            // Frame sequence
            spriteRenderer.sprite = sprites[0];
            yield return frameDelay;
            yield return frameDelay;

            spriteRenderer.sprite = sprites[1];
            yield return frameDelay;

            spriteRenderer.sprite = sprites[2];

            // Apply damage if ball is not shielded
            if (!GameManager.Instance.IsShieldActive())
                EventBus.RaiseGameOver();

            yield return frameDelay;
            spriteRenderer.sprite = sprites[0];
        }
    }
}
