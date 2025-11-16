using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class SplashScene : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float fadeDuration = 1.5f;   // n seconds
    public float holdDuration = 1.0f;   // m seconds

    private void Start()
    {
        StartCoroutine(FadeHSV());
    }

    private IEnumerator FadeHSV()
    {
        yield return new WaitForSeconds(0.5f);


        Color original = spriteRenderer.color;
        Color.RGBToHSV(original, out float h, out float s, out float v);

        // Fade V: 1 → 0
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float vOut = Mathf.Lerp(0f, 1f, t / fadeDuration);
            spriteRenderer.color = Color.HSVToRGB(h, s, vOut);
            yield return null;
        }

        // Hold
        yield return new WaitForSeconds(holdDuration);

        // Fade V: 0 → 1
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float vOut = Mathf.Lerp(1f, 0f, t / fadeDuration);
            spriteRenderer.color = Color.HSVToRGB(h, s, vOut);
            yield return null;
        }

        // Restore exact original at the end
        spriteRenderer.color = original;


        Scene current = SceneManager.GetActiveScene();

        // Load next scene
        SceneManager.LoadScene("InitialScene");

        // Unload previous scene
        SceneManager.UnloadSceneAsync(current);
    }
}
