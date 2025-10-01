using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class Rocket : MonoBehaviour,IHitBall
{

    public void OnHitBall(BallController ballController)
    {
       EventBus.RaiseGameOver();
    }
}
