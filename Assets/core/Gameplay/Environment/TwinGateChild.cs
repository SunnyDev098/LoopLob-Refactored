using UnityEngine;
using Gameplay.Player;
using Gameplay.Interfaces;

public class TwinGateChild : MonoBehaviour
{
    private TwinGate parentGate;

    private void Awake()
    {
        parentGate = GetComponentInParent<TwinGate>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<BallController>(out var ball))
            return;

        parentGate?.OnTeleport(ball);
    }
}
