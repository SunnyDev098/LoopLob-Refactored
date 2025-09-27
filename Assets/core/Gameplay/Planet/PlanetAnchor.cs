using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class PlanetAnchor : MonoBehaviour, IHitBall
{
    [SerializeField] private GameObject scoreObject;
    private bool isItFirstAttach;
    private void Start()
    {
        isItFirstAttach = true;
    }
    public void OnHitBall(BallController ball)
    {
       
        ball.AnchorTo(transform);
        ball.SetOrbitRadius(GetComponent<PlanetAttribute>().orbitRadius);
        GetComponent<PlanetAttribute>().ActivateOrbitVisual();

        if (isItFirstAttach)
        {
            isItFirstAttach =false;
            createScore();
        }

    }

    private void createScore()
    {
        Instantiate(scoreObject, transform.position, Quaternion.identity,transform);
    }
}
