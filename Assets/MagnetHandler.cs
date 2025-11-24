using System.Collections;
using UnityEngine;

public class MagnetHandler : MonoBehaviour
{
    [Header("Magnet Settings")]
    public Transform target;
    public float startSpeed = 2f;
    public float endSpeed = 10f;
    public float accelerationTime = 1.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            // Prevent starting multiple coroutines on the same coin
            CoinMagnetMover mover = other.GetComponent<CoinMagnetMover>();
            if (mover == null)
            {
                mover = other.gameObject.AddComponent<CoinMagnetMover>();
            }
            mover.StartPull(target, startSpeed, endSpeed, accelerationTime);
        }
    }
}


/// <summary>
/// Handles independent smooth attraction for a single coin.
/// Prevents coroutine interference when multiple coins are pulled simultaneously.
/// </summary>
public class CoinMagnetMover : MonoBehaviour
{
    private Transform target;
    private float startSpeed;
    private float endSpeed;
    private float accelerationTime;
    private Coroutine pullRoutine;

    public void StartPull(Transform target, float startSpeed, float endSpeed, float accelerationTime)
    {
        this.target = target;
        this.startSpeed = startSpeed;
        this.endSpeed = endSpeed;
        this.accelerationTime = accelerationTime;

        // if a coroutine is already running on this coin, stop it before starting a new one
        if (pullRoutine != null)
            StopCoroutine(pullRoutine);

        pullRoutine = StartCoroutine(Pull());
    }

    private IEnumerator Pull()
    {
        if (target == null) yield break;

        float elapsed = 0f;
        float speed = startSpeed;

        while (true)
        {
            if (target == null) yield break;

            // interpolate speed
            if (elapsed < accelerationTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / accelerationTime);
                speed = Mathf.Lerp(startSpeed, endSpeed, t);
            }

            // move coin
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;

            // if reached target
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < 0.1f)
            {
                Destroy(gameObject); // or call collect event
                yield break;
            }

            yield return null;
        }
    }

    private void OnDisable()
    {
        if (pullRoutine != null)
            StopCoroutine(pullRoutine);
    }
}
