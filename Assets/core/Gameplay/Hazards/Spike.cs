using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class Spike : MonoBehaviour, IHazard
{

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, 100 * Time.deltaTime);

    }
    public void OnHit(BallController ball)
    {
        // Simple example: End the game
        EventBus.RaiseGameOver();
    }
}
