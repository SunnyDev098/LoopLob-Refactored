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
        // Start delayed collision check
        Invoke(nameof(CheckInitialOverlap), selfCheckDelay);
    }

    private void CheckInitialOverlap()
    {
        // Collect all colliders that overlap this coin's current position
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (var hit in hits)
        {
            // Ignore self
            if (hit.gameObject == gameObject) continue;

            // If there's any other collider nearby (trigger or not), remove the coin
            if (hit.enabled)
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
