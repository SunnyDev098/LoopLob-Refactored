using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.UIElements;

public class Rocket : MonoBehaviour,IHitBall
{
  
    public void OnHitBall(BallController ballController)
    {
        Core.EventBus.RaiseGameOver();
    }
}
