using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class PlanetAnchor : MonoBehaviour, IHitBall
{
    public void OnHitBall(BallController ball)
    {
        ball.AnchorTo(transform);
        ball.SetOrbitRadius(GetComponent<PlanetAttribute>().orbitRadius);
        GetComponent<PlanetAttribute>().ActivateOrbitVisual();

    }
}
