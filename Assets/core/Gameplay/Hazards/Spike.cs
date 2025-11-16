using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class Spike : MonoBehaviour, IHitBall
{

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, 100 * Time.deltaTime);
        // some new change

    }
    public void OnHitBall(BallController ball)
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
