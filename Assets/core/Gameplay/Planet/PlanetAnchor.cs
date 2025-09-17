using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class PlanetAnchor : MonoBehaviour, IOrbitAnchor
{
    public void OnAnchored(BallController ball)
    {
        ball.AnchorTo(transform);
        ball.SetOrbitRadius(GetComponent<PlanetAttribute>().orbitRadius);
        GetComponent<PlanetAttribute>().ActivateOrbitVisual();

    }
}
