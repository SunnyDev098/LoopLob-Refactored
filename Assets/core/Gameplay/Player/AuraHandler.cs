namespace Gameplay.Player
{
    using UnityEngine;
    using System.Collections;
    using Core; // for EventBus

    /// <summary>
    /// Handles an aura-like color transition on a referenced SpriteRenderer.
    /// Animates color from white to red over a time period, then triggers GameOver.
    /// </summary>
    public class AuraHandler : MonoBehaviour
    {
        [Header("Aura Settings")]
        [SerializeField] private SpriteRenderer targetRenderer;       // SpriteRenderer to colorize
        [SerializeField] public float duration = 4f;                 // Time to reach target color
        [SerializeField] private Color startColor = Color.white;      // Starting color
        [SerializeField] private Color targetColor = Color.red;       // Target color
        [SerializeField] private bool triggerGameOverOnLimit = true;  // Optional game-over trigger

        private Coroutine auraRoutine;

        private void Awake()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Starts the color animation on the target SpriteRenderer.
        /// </summary>
        public void StartAura()
        {
            if (auraRoutine != null)
                StopCoroutine(auraRoutine);

            auraRoutine = StartCoroutine(AuraAnimationRoutine());
        }

        /// <summary>
        /// Instantly resets color to the start color.
        /// </summary>
        public void ResetAura()
        {

            if (auraRoutine != null)
            {
                StopCoroutine(auraRoutine);
                auraRoutine = null;
            }

            if (targetRenderer != null)
                targetRenderer.color = startColor;
        }

        private IEnumerator AuraAnimationRoutine()
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                // Support pause handling (optional)
                if (GameManager.Instance != null && GameManager.Instance.IsGamePaused())
                {
                    yield return null;
                    continue;
                }

                float t = elapsed / duration;

                // Interpolate color from white → red
                if (targetRenderer != null)
                    targetRenderer.color = Color.Lerp(startColor, targetColor, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure final color
            if (targetRenderer != null)
                targetRenderer.color = targetColor;

            // EventBus trigger
            if (triggerGameOverOnLimit)
                targetRenderer.color = startColor;


            if (GameManager.Instance.IsShieldActive())
            {
                ResetAura();
                GameManager.Instance.DeActiveSheildCall();
            }
            else
            {
                EventBus.RaiseGameOver();

            }
        }
    }
}
