using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class Spike : MonoBehaviour, IHazard
{
    public void OnHit(BallController ball)
    {
        // Simple example: End the game
        GameManager.Instance.EndGame(GameManager.Instance.GetCurrentScore());
    }
}
