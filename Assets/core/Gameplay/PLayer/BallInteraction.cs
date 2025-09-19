using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class BallInteraction : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.TryGetComponent<IHazard>(out var hazard))
            hazard.OnHit(GetComponent<BallController>());

        if (other.transform.TryGetComponent<ICollectible>(out var collectible))
            collectible.OnCollected(GetComponent<BallController>());


        if (other.transform.TryGetComponent<IOrbitAnchor>(out var anchor))
            anchor.OnAnchored(GetComponent<BallController>());

        if (other.transform.TryGetComponent<ITwinGate>(out var teleporter))
            teleporter.OnTeleport(GetComponent<BallController>());

    }
}
