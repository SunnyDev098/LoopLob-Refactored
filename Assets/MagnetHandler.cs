using System.Collections;
using UnityEngine;

public class MagnetHandler : MonoBehaviour
{
    [Header("Magnet Settings")]
    public Transform target;           // Player or magnet center
    public float startSpeed = 2f;      // initial pull speed (x)
    public float endSpeed = 10f;       // final pull speed (y)
    public float accelerationTime = 1.5f; // time (t) to reach full pull speed

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            // Start the coroutine for this specific coin
            StartCoroutine(PullCoinTowardsTarget(other.gameObject));
        }
    }

    private IEnumerator PullCoinTowardsTarget(GameObject coin)
    {
        if (coin == null || target == null)
            yield break;

        float elapsed = 0f;
        float speed = startSpeed;

        // continue pulling until coin is collected or destroyed
        while (coin != null)
        {
            if (target == null)
                yield break;

            // gradually interpolate the speed from startSpeed → endSpeed over accelerationTime
            if (elapsed < accelerationTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / accelerationTime);
                speed = Mathf.Lerp(startSpeed, endSpeed, t);
            }

            // move coin toward the target
            Vector3 dir = (target.position - coin.transform.position).normalized;
            coin.transform.position += dir * speed * Time.deltaTime;

            float distance = Vector3.Distance(coin.transform.position, target.position);
         

            yield return null;
        }
    }
}
