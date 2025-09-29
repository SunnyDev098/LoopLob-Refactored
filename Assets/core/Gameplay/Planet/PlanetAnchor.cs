using Gameplay.Interfaces;
using Gameplay.Player;
using TMPro;
using UnityEngine;

public class PlanetAnchor : MonoBehaviour, IHitBall
{
    [SerializeField] private GameObject scoreObject;
    private bool isItFirstAttach;
    private static int lastScoreBallY;

  
    private void Start()
    {
        isItFirstAttach = true;
        lastScoreBallY = 0;
    }
    public void OnHitBall(BallController ball)
    {
        Debug.Log("weeeeeeeeee");
        ball.AnchorTo(transform);
        ball.SetOrbitRadius(GetComponent<PlanetAttribute>().orbitRadius);
        GetComponent<PlanetAttribute>().ActivateOrbitVisual();

        if (isItFirstAttach)
        {
            isItFirstAttach =false;

            int currentBallY = (int)ball.transform.position.y;
            if (currentBallY > lastScoreBallY)
            {
                createScore((currentBallY - lastScoreBallY));
                lastScoreBallY = currentBallY;
                Core.EventBus.RaiseScoreChanged(currentBallY);


            }
           
        }

    }

    private void createScore(int plusScore)
    {
       GameObject madedScore = Instantiate(scoreObject, transform.position, Quaternion.identity);
        madedScore.gameObject.GetComponent<ScoreObjectHandler>().text1.text ="+"+ plusScore.ToString();
    }
}
