using UnityEngine;
using System.Collections;

namespace Gameplay.player
{
    public class SpritePulseController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Assign exactly 2 sprite renderers for left/right visuals.")]
        public SpriteRenderer[] spriteRenderers = new SpriteRenderer[2];

        [Header("Pulse Settings")]
        [SerializeField] private float minAlpha = 0f;
        [SerializeField] private float maxAlpha = 20f / 255f;
        [SerializeField] private float minScaleX = 2f;
        [SerializeField] private float maxScaleX = 3.5f;
        [SerializeField] private float returnDuration = 0.25f;

        // Tracks the per-renderer return coroutines
        private Coroutine[] returnCoroutines = new Coroutine[2];

        /// <summary>
        /// Triggers a pulse effect on the specified sprite index (0 or 1).
        /// </summary>
        public void PlayPulse(int spriteIndex)
        {
            if (!IsValidIndex(spriteIndex) || spriteRenderers[spriteIndex] == null)
                return;

            SetAlpha(spriteIndex, maxAlpha);
            SetScaleX(spriteIndex, maxScaleX);

            // Stop any ongoing return animation
            if (returnCoroutines[spriteIndex] != null)
                StopCoroutine(returnCoroutines[spriteIndex]);

            // Start a new return animation
            returnCoroutines[spriteIndex] = StartCoroutine(ReturnToRest(spriteIndex));
        }

        /// <summary>
        /// Smoothly returns the sprite from pulse state back to rest state.
        /// </summary>
        private IEnumerator ReturnToRest(int spriteIndex)
        {
            var renderer = spriteRenderers[spriteIndex];
            float t = 0;
            float startAlpha = renderer.color.a;
            float startScaleX = renderer.transform.localScale.x;

            while (t < returnDuration)
            {
                t += Time.deltaTime;
                float normalized = Mathf.Clamp01(t / returnDuration);

                SetAlpha(spriteIndex, Mathf.Lerp(startAlpha, minAlpha, normalized));
                SetScaleX(spriteIndex, Mathf.Lerp(startScaleX, minScaleX, normalized));

                yield return null;
            }

            SetAlpha(spriteIndex, minAlpha);
            SetScaleX(spriteIndex, minScaleX);
            returnCoroutines[spriteIndex] = null;
        }

        private void SetAlpha(int idx, float alpha)
        {
            var c = spriteRenderers[idx].color;
            c.a = alpha;
            spriteRenderers[idx].color = c;
        }

        private void SetScaleX(int idx, float x)
        {
            var s = spriteRenderers[idx].transform.localScale;
            s.x = x;
            spriteRenderers[idx].transform.localScale = s;
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < spriteRenderers.Length;
        }
    }

}


