using UnityEngine;
using System.Collections;
using Gameplay.Interfaces;
using Gameplay.Player;
using Core;

namespace Gameplay.Player
{
    /// Alien enemy that wanders in a PingPong pattern and cycles sprites when hitting the ball.
    /// Damage application is routed through the IHitBall interface.
    
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
            startPosition = new Vector3(0, transform.position.y, transform.position.z) ;

            if (movementCurve == null || movementCurve.length == 0)
                movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        private void Update()
        {
            UpdateMovement();
        }

    
        public void OnHitBall(BallController ballController)
        {


            if (GameManager.Instance.isShieldActive)
            {
                GameManager.Instance.DeActiveSheildCall();
            }
            else
            {
                EventBus.RaiseGameOver();

            }
        }

        private void UpdateMovement()
        {
            // Horizontal motion
            tX += Time.deltaTime / Mathf.Max(0.0001f, xDuration);
            float xOffset = Mathf.Lerp(-xRange, xRange, movementCurve.Evaluate(Mathf.PingPong(tX, 1f)));

            // Vertical motion
            tY += Time.deltaTime / Mathf.Max(0.0001f, yDuration);
            float yOffset = Mathf.Lerp(-yRange, yRange, movementCurve.Evaluate(Mathf.PingPong(tY, 1f)));

            transform.position =  startPosition + new Vector3(xOffset, yOffset, 0f);
        }

      

        private IEnumerator CycleSpritesCoroutine()
        {

            spriteRenderer.sprite = sprites[0];

            yield return frameDelay;

            spriteRenderer.sprite = sprites[1];

            yield return frameDelay;

            spriteRenderer.sprite = sprites[2];
        

            if (!GameManager.Instance.IsShieldActive())
                EventBus.RaiseGameOver();

        }
    }
}
