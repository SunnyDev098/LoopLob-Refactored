using Core;
using Gameplay.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerUpManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button buttonShield;
    public Button buttonFuel;
    public Button buttonMagnet;

    [Header("Prices")]
    private int extraFuelPrice = 20;
    private int shieldPrice = 30;
    private int magnetPrice = 25;

    [Header("Price Texts")]
    public TextMeshProUGUI shieldPriceTxt;
    public TextMeshProUGUI fuelPriceTxt;
    public TextMeshProUGUI magnetPriceTxt;

    [Header("GameObjects")]
    public GameObject Shield;
    public GameObject Magnet; // your magnet visual or effect

    [Header("References")]
    public AuraHandler auraHandler;
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip shield;
    public AudioClip error;
    public AudioClip magnet; // assign new magnet audio

    private Vector3 original_scale;

    void Start()
    {
        shieldPriceTxt.text = shieldPrice.ToString();
        fuelPriceTxt.text = extraFuelPrice.ToString();
        magnetPriceTxt.text = magnetPrice.ToString();

        buttonShield.onClick.AddListener(OnShieldClick);
        buttonFuel.onClick.AddListener(OnFuelClick);
        buttonMagnet.onClick.AddListener(OnMagnetClick);

        audioSource = gameObject.AddComponent<AudioSource>();
        original_scale = buttonFuel.transform.localScale;
    }

    void OnShieldClick()
    {
        if (DataHandler.Instance.GetTotalCoins() >= shieldPrice)
        {
            buttonShield.gameObject.SetActive(false);
            GameManager.Instance.isShieldActive = true;
            Shield.SetActive(true);
            audioSource.PlayOneShot(shield);

            DataHandler.Instance.SaveTotalCoins(DataHandler.Instance.GetTotalCoins() - shieldPrice);
            GameManager.Instance.coinNumber = DataHandler.Instance.GetTotalCoins();
            GameManager.Instance.AddCoin(0);
        }
        else
        {
            StartCoroutine(ScaleAndColor(buttonShield.transform, shieldPriceTxt, 1.1f, 0.25f));
            audioSource.PlayOneShot(error);
        }
    }

    void OnFuelClick()
    {
        if (DataHandler.Instance.GetTotalCoins() >= extraFuelPrice)
        {
            buttonFuel.gameObject.SetActive(false);
            auraHandler.duration = 6;
            audioSource.PlayOneShot(shield);

            DataHandler.Instance.SaveTotalCoins(DataHandler.Instance.GetTotalCoins() - extraFuelPrice);
            GameManager.Instance.coinNumber = DataHandler.Instance.GetTotalCoins();
            GameManager.Instance.AddCoin(0);
        }
        else
        {
            StartCoroutine(ScaleAndColor(buttonFuel.transform, fuelPriceTxt, 1.1f, 0.25f));
            audioSource.PlayOneShot(error);
        }
    }

    void OnMagnetClick()
    {
        if (DataHandler.Instance.GetTotalCoins() >= magnetPrice)
        {
            buttonMagnet.gameObject.SetActive(false);

            // --- activate magnet logic
            GameManager.Instance.isMagnetActive = true;

            if (Magnet != null)
                Magnet.SetActive(true); // e.g., visual magnet field

            audioSource.PlayOneShot(shield);

            DataHandler.Instance.SaveTotalCoins(DataHandler.Instance.GetTotalCoins() - magnetPrice);
            GameManager.Instance.coinNumber = DataHandler.Instance.GetTotalCoins();
            GameManager.Instance.AddCoin(0);
        }
        else
        {
            StartCoroutine(ScaleAndColor(buttonMagnet.transform, magnetPriceTxt, 1.1f, 0.25f));
            audioSource.PlayOneShot(error);
        }
    }

    public IEnumerator ScaleAndColor(Transform target, TMP_Text text, float scaleMultiplier, float duration)
    {
        target.localScale = original_scale;
        text.color = Color.white;

        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale * scaleMultiplier;

        Color originalColor = text.color;
        Color.RGBToHSV(originalColor, out float h, out float s, out float v);

        float halfDuration = duration / 2f;
        float timer = 0f;

        // Phase 1: Scale up & intensify
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            target.localScale = Vector3.Lerp(originalScale, targetScale, t);

            float newS = Mathf.Lerp(s, 1f, t);
            text.color = Color.HSVToRGB(h, newS, v);

            yield return null;
        }

        // Phase 2: Scale down
        timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            target.localScale = Vector3.Lerp(targetScale, originalScale, t);

            float newS = Mathf.Lerp(1f, s, t);
            text.color = Color.HSVToRGB(h, newS, v);
            yield return null;
        }

        target.localScale = originalScale;
        text.color = originalColor;
    }
}
