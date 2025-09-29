using Core;
using Gameplay.Interfaces;
using Gameplay.Player;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class Rocket : MonoBehaviour,IHitBall
{
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        if (audioSource != null)
            audioSource.volume = PlayerPrefs.GetFloat("sfx_volume");

    }
    public void OnHitBall(BallController ballController)
    {
        Core.EventBus.RaiseGameOver();
    }
}
