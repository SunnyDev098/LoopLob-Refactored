using System.Collections.Generic;
using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class Spike : MonoBehaviour, IHitBall
{
    public List<Sprite> possibleSprite;

    private void Start()
    {
        if (possibleSprite == null || possibleSprite.Count < 4)
            return;

        int randomNum = Random.Range(1, 5); // 1–4 inclusive
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector3 scale = transform.localScale;

        switch (randomNum)
        {
            case 1:
                sr.sprite = possibleSprite[0];
                transform.localScale = scale * 0.7f;
                break;
            case 2:
                sr.sprite = possibleSprite[1];
                transform.localScale = scale * 0.85f;
                break;
            case 3:
                sr.sprite = possibleSprite[2];
                transform.localScale = scale * 1f;
                break;
            case 4:
                sr.sprite = possibleSprite[3];
                transform.localScale = scale * 1.15f;
                break;
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, 100 * Time.deltaTime);
        // some new change
    }

    public void OnHitBall(BallController ball)
    {
        if (GameManager.Instance.isShieldActive)
            GameManager.Instance.DeActiveSheildCall();
        else
            EventBus.RaiseGameOver();
    }
}
