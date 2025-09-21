using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class Coin : MonoBehaviour, IHitBall
{
    [SerializeField] private int value = 1;

    public void OnHitBall(BallController ball)
    {
        GameManager.Instance.AddCoin(value);
        Destroy(gameObject);
    }
}
