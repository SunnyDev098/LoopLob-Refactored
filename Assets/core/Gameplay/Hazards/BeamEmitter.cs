using Core;
using UnityEngine;
using Gameplay.Player;
using Gameplay.Interfaces;
using UnityEditor.UI;
/// <summary>
/// Cycles through a list of sprites at a fixed interval to create a beam animation.
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(AudioSource))]
public class BeamEmitter : MonoBehaviour,IHazard
{
    [Header("Animation Settings")]
    [Tooltip("Sprites to cycle through for the animation.")]
    [SerializeField] private Sprite[] sprites;

    [Tooltip("Time in seconds between sprite changes.")]
    [SerializeField] private float interval = 0.05f;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private int currentIndex;
    private float timer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetInitialVolume();
        SetInitialSprite();
    }

    private void Update()
    {
        if (!HasSpritesAssigned()) return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer -= interval;
            AdvanceSprite();
        }
    }

    private void SetInitialVolume()
    {
        if (audioSource != null)
            audioSource.volume *= GameManager.Instance.SfxVolume * 3f;
    }

    private void SetInitialSprite()
    {
        if (HasSpritesAssigned())
            spriteRenderer.sprite = sprites[0];
    }

    private void AdvanceSprite()
    {
        currentIndex = (currentIndex + 1) % sprites.Length;
        spriteRenderer.sprite = sprites[currentIndex];
    }

    private bool HasSpritesAssigned()
    {
        return sprites != null && sprites.Length > 0;
    }

    

    public BeamEmitter MakeBeamEmitter(Transform pos)
    {
        BeamEmitter madedBeamEmitter = Instantiate(this, pos.position, Quaternion.identity);
        return madedBeamEmitter;
    }
    public void OnHit(BallController ball)
    {
        EventBus.RaiseGameOver();
    }
}
