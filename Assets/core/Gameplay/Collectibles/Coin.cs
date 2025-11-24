using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class Coin : MonoBehaviour, IHitBall
{
    [SerializeField] private int value = 1;
    [SerializeField] private float selfCheckDelay = 1f;

    private void Start()
    {
        Invoke("CheckInitialOverlap", 1);
    }

    private void CheckInitialOverlap()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (hit.enabled && hit.CompareTag("OuterOrbit"))
            {
                Destroy(gameObject);
                return;
            }

            if (hit.enabled && hit.CompareTag("emitterAura"))
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    public void OnHitBall(BallController ball)
    {
        ball.GetCoin();
        Destroy(gameObject);
    }
}
