using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreObjectHandler : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    private Vector3 move_vector = new Vector3(1, 1, 0);
    // Start fading immediately (or call StartFade from any event)
    void Start()
    {
        StartFade();
        Destroy(gameObject, 3);
    }

    public void StartFade()
    {
        StartCoroutine(FadeTextAlpha(text1, 255f / 255f, 0f, 1.5f));
        StartCoroutine(FadeTextAlpha(text2, 255f / 255f, 0f, 1.5f));
    }

    IEnumerator FadeTextAlpha(TextMeshProUGUI text, float startAlpha, float endAlpha, float duration)
    {
        if (text == null) yield break;

        Color originalColor = text.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = transform.position + move_vector * Time.deltaTime;

            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Ensure fully faded after loop
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }
}
