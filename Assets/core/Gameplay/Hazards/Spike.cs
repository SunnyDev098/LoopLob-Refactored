using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class Spike : MonoBehaviour, IHazard
{

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, 100 * Time.deltaTime);
        // some new change
        // some new change2
        // some new change3
        // some new change44
        // some new change5
    }
    public void OnHit(BallController ball)
    {
        // Simple example: End the game
        EventBus.RaiseGameOver();
    }
}
