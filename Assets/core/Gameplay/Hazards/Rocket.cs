using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class Rocket : MonoBehaviour,IHitBall
{

    public void OnHitBall(BallController ballController)
    {

        if (GameManager.Instance.isShieldActive)
        {
            GameManager.Instance.DeActiveSheildCall();
        }
        else
        {
            EventBus.RaiseGameOver();

        }
    }
}
