using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class BallInteraction : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(GameManager.Instance.IsBallAttached())return;
        if(GameManager.Instance.IsDebugMode)return;
        if (other.transform.TryGetComponent<IHitBall>(out var otherThing))
            otherThing.OnHitBall(GetComponent<BallController>());
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (GameManager.Instance.IsBallAttached()) return;
        if (GameManager.Instance.IsDebugMode) return;
        if (other.TryGetComponent<IAttractor>(out var blackHoleThing))
            blackHoleThing.OnAttracted(GetComponent<BallController>());
    }
}
