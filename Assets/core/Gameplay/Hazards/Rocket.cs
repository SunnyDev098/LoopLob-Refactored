using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class Rocket : MonoBehaviour,IHitBall
{

    public void OnHitBall(BallController ballController)
    {
        Core.EventBus.RaiseGameOver();
    }
}
