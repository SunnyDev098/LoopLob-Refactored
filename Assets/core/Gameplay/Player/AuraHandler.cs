namespace Gameplay.player
{
    using UnityEngine;
    using System.Collections;
    using Core; // for EventBus
    using Core; // ensure you have access to GameManager if needed

    /// <summary>
    /// Handles an aura effect around an object by animating scale and alpha.
    /// Triggers a GameOver event when the animation completes.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class AuraHandler : MonoBehaviour
    {
        [Header("Aura Settings")]
        [SerializeField] private float duration = 1f;                // Total time to reach target
        [SerializeField] private float targetScale = 1.5f;            // Scale multiplier at peak
        [SerializeField, Range(0f, 1f)] private float targetAlpha = 1f; // Alpha at peak
        [SerializeField] private bool triggerGameOverOnLimit = true;  // End game when aura completes
      

        private SpriteRenderer spriteRenderer;
        private Vector3 initialScale;
        private float initialAlpha;
        private Coroutine auraRoutine;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            initialScale = transform.localScale;
            initialAlpha = spriteRenderer.color.a;
        }

        /// <summary>
        /// Starts the aura animation.
        /// </summary>
        public void StartAura()
        {
            if (auraRoutine != null)
                StopCoroutine(auraRoutine);

            auraRoutine = StartCoroutine(AuraAnimationRoutine());
        }

        /// <summary>
        /// Resets aura to its original scale and alpha instantly.
        /// </summary>
        public void ResetAura()
        {
            if (auraRoutine != null)
            {
                StopCoroutine(auraRoutine);
                auraRoutine = null;
            }

            transform.localScale = initialScale;
            SetAlpha(initialAlpha);
        }

        private IEnumerator AuraAnimationRoutine()
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                // Optional pause support
                if (GameManager.Instance != null && GameManager.Instance.IsGamePaused())
                {
                    yield return null;
                    continue;
                }

                float t = elapsed / duration;

                // Scale interpolation
                transform.localScale = Vector3.Lerp(initialScale, initialScale * targetScale, t);

                // Alpha interpolation
                float alphaValue = Mathf.Lerp(initialAlpha, targetAlpha, t);
                SetAlpha(alphaValue);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure final state
            transform.localScale = initialScale * targetScale;
            SetAlpha(targetAlpha);

            // Trigger game-over via EventBus
            if (triggerGameOverOnLimit)
            {
              //  EventBus.RaiseGameOver();
            
            }
        }

        private void SetAlpha(float alpha)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }
    }
}
