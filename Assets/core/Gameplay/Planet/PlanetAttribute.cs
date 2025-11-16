using Gameplay.Player;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Core;


public class PlanetAttribute : MonoBehaviour
{
    [Header("Planet Type")]
    [Tooltip("If true, this planet has a limited lifespan when player attaches")]
    [SerializeField] private bool isBadPlanet = false;

    [Tooltip("Whether the planet moves horizontally")]
    [SerializeField] private bool isMoving = false;

    [Header("Orbit Settings")]
    [Tooltip("Orbit visual GameObject (optional)")]
    [SerializeField] private GameObject activatedOrbit;
    [SerializeField] private GameObject ExplosionVfx;

    [Tooltip("Orbit radius in world units, based on planet size")]
    public float orbitRadius { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private float xRange = 3f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Bad Planet Settings")]
    [Tooltip("Lifespan slider UI for bad planets")]
    [SerializeField] private GameObject lifespanSliderObject;
    [SerializeField] private Slider lifespanSlider;
    [SerializeField] private float badPlanetLifespan = 3f;
    [SerializeField] private Sprite BadPlanetSprite;

    // Properties
    public bool IsBadPlanet => isBadPlanet;
    public bool IsMoving => isMoving;

    // Private fields
    private Vector3 startPosition;
    private float minXLimit;
    private float maxXLimit;
    private float moveTimer;
    private Coroutine lifespanCoroutine;

    public bool IsAnchoreToBall = false;

    private void Start()
    {
        InitializePlanet();
    }

    private void Update()
    {
        if (isMoving)
            UpdateMovement();
    }



    private void InitializePlanet()
    {
        SetupMovementLimits();
        CalculateOrbitRadius();
        HideOrbitVisual();
        if (IsBadPlanet) GetComponent<SpriteRenderer>().sprite = BadPlanetSprite;

    }

    private void SetupMovementLimits()
    {
        minXLimit = -xRange;
        maxXLimit = xRange;
        startPosition = transform.position;
    }

    private void CalculateOrbitRadius()
    {
        orbitRadius = (transform.localScale.x * 1.5f) / 2f;
    }

    public void HideOrbitVisual()
    {
        if (activatedOrbit != null)
            activatedOrbit.SetActive(false);
    }

    

  
    /// <summary>
    /// Updates horizontal oscillation movement.
    /// </summary>
    private void UpdateMovement()
    {
        moveTimer += Time.deltaTime;
        float normalizedTime = Mathf.PingPong(moveTimer, 1f);
        float curvedValue = movementCurve.Evaluate(normalizedTime);
        float horizontalOffset = Mathf.Lerp(minXLimit, maxXLimit, curvedValue);

        transform.position = startPosition + new Vector3(horizontalOffset, 0f, 0f);
    }

    /// Toggles the planet's movement direction.
    public void ToggleMovementDirection()
    {
        xRange = -xRange;
        minXLimit = -xRange;
        maxXLimit = xRange;
    }

   

    /// Activates the orbit visual with a scale animation.
    public void ActivateOrbitVisual()
    {
        if (activatedOrbit == null) return;

        if (!isBadPlanet)
        {
            activatedOrbit.SetActive(true);
            StartCoroutine(AnimateOrbitScaleRoutine(0.2f));
        }
        else
        {
            ActivateBadPlanetBehavior();
        }

    }

    private IEnumerator AnimateOrbitScaleRoutine(float duration)
    {
        Vector3 targetScale = activatedOrbit.transform.localScale;
        activatedOrbit.transform.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            activatedOrbit.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, progress);
            yield return null;
        }

        activatedOrbit.transform.localScale = targetScale;
    }

   

   
   



    private void ActivateBadPlanetBehavior()
    {
        ShowLifespanSlider();
        StartLifespanCountdown();
    }

 
    private void ShowLifespanSlider()
    {
        if (lifespanSliderObject == null) return;

        lifespanSliderObject.SetActive(true);

        if (lifespanSlider != null)
        {
            lifespanSlider.maxValue = 1f;
            lifespanSlider.value = 1f;
        }
    }

    private void StartLifespanCountdown()
    {
        if (lifespanCoroutine != null)
            StopCoroutine(lifespanCoroutine);

        lifespanCoroutine = StartCoroutine(LifespanCountdownRoutine());
    }

   

    private IEnumerator LifespanCountdownRoutine()
    {
        float elapsed = 0f;

        while (elapsed < badPlanetLifespan)
        {
            elapsed += Time.deltaTime;
            float remainingLifePercentage = 1f - (elapsed / badPlanetLifespan);

            UpdateLifespanSlider(remainingLifePercentage);

            yield return null;
        }

        OnLifespanExpired();
    }

    private void UpdateLifespanSlider(float value)
    {
        if (lifespanSlider != null)
            lifespanSlider.value = Mathf.Clamp01(value);
    }

    private void OnLifespanExpired()
    {
        GameObject explosionGO = Instantiate(ExplosionVfx,transform.position, Quaternion.identity, transform);

        if (!GameManager.Instance.IsBallAttached()) return;
        if (!IsAnchoreToBall) return;

        EventBus.RaiseGameOver();


    }
    public void SetBadPlanet(bool isBad)
    {
        isBadPlanet = isBad;

    }


}
