using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class Coin : MonoBehaviour, ICollectible
{
    [SerializeField] private int value = 1;

    public void OnCollected(BallController ball)
    {
        GameManager.Instance.AddCoin(value);
        Destroy(gameObject);
    }
}
