using Core;
using UnityEngine;
using Gameplay.Player;
using Gameplay.Interfaces;

public class BeamEmitter : MonoBehaviour, IHitBall
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
        SetInitialSprite();

        Invoke("CheckInitialOverlap", 1);
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


   


    private void CheckInitialOverlap()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (hit.enabled && hit.CompareTag("OuterOrbit"))
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
