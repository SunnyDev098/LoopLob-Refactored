using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlanetAnchor : MonoBehaviour, IHitBall
{
    [SerializeField] private GameObject scoreObject;
    private bool isItFirstAttach;
    private  int lastScoreBallY;
    private  int currentBallY;

  
    private void Start()
    {
        isItFirstAttach = true;


    }
    public  void OnHitBall(BallController ball)
    {
        GameManager.Instance.AttachedPlanet = transform.gameObject;
        ball.AnchorTo(transform);
        ball.SetOrbitRadius(GetComponent<PlanetAttribute>().orbitRadius);
        GetComponent<PlanetAttribute>().ActivateOrbitVisual();

        if (isItFirstAttach)
        {
          //  Debug.Log(lastScoreBallY);
          //  Debug.Log((int)ball.transform.position.y);
            isItFirstAttach =false;

            currentBallY = (int)ball.transform.position.y;
            if (currentBallY > lastScoreBallY)
            {
                
                createScore((int)transform.position.y -GameManager.Instance.lastbally);

                GameManager.Instance.increaseCurrentScore(((int)transform.position.y - GameManager.Instance.lastbally));
                EventBus.RaiseScoreChanged((GameManager.Instance.GetCurrentScore()));
                GameManager.Instance.lastbally = (int)transform.position.y;
            }
           
        }
        lastScoreBallY = currentBallY;

    }

    private void createScore(int plusScore)
    {
       GameObject madedScore = Instantiate(scoreObject, transform.position, Quaternion.identity);
        madedScore.gameObject.GetComponent<ScoreObjectHandler>().text1.text ="+"+ plusScore.ToString();
    }
}
