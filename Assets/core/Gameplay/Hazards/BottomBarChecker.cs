using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;
using Core;

public class BottomBarChecker : MonoBehaviour,IHitBall
{
    public void OnHitBall(BallController ball)
    {
        EventBus.RaiseGameOver();
    }
}
