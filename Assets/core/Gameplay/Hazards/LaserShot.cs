using Gameplay.Player;
using Gameplay.Interfaces;
using Core;
using UnityEngine;

public class LaserShot : MonoBehaviour, IHitBall
{
     
    public void OnHitBall(BallController ballController)
    {
        EventBus.RaiseGameOver();
    }
}
