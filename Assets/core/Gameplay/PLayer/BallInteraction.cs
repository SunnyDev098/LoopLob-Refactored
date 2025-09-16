using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class BallInteraction : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent<IHazard>(out var hazard))
            hazard.OnHit(GetComponent<BallController>());

        if (collision.transform.TryGetComponent<ICollectible>(out var collectible))
            collectible.OnCollected(GetComponent<BallController>());
    }
}
