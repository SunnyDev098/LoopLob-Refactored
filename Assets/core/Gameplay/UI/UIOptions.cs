using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button BackButton;

    [SerializeField] private Button BackGroundOptionsButton;
    [SerializeField] private Button SellectBackGroundButton;

    public TextMeshProUGUI BackChangeText;



    [SerializeField] private Button LeftBackGroundButton;
    [SerializeField] private Button RightBackGroundButton;

    [SerializeField] private Button BackToSettingsButton;

    [SerializeField] private MainMenuHandler mainMenuHandler;
    [SerializeField] private AudioMixer  audioMixer;
     private int temp_BG_index=0;

    private const string MUSIC_PREF_KEY = "MusicVolume";
    private const string SFX_PREF_KEY = "SFXVolume";

    public CameraBackGroundHandler cameraBackGroundHandler;

    public List<GameObject> ToHideObjects;

    private void Awake()
    {
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_PREF_KEY);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_PREF_KEY);


        musicSlider.onValueChanged.AddListener(OnMusicVolumeChange);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChange);
        BackButton.onClick.AddListener(OnBackButtonClick);
        BackGroundOptionsButton.onClick.AddListener(OnBackGroundOptionsButtonClick);

        LeftBackGroundButton.onClick.AddListener(OnLeftBackGroundButtonClick);
        RightBackGroundButton.onClick.AddListener(OnRightBackGroundButtonClick);

        BackToSettingsButton.onClick.AddListener(OnBackToSettingsButtonClick);


        SellectBackGroundButton.onClick.AddListener(OnSellectBackGroundButtonClick);


    }

    private void OnMusicVolumeChange(float value)
    {
        PlayerPrefs.SetFloat(MUSIC_PREF_KEY, value);
        PlayerPrefs.Save();
        float dbValue = Mathf.Lerp(-30f, 10f, value);
        audioMixer.SetFloat(MUSIC_PREF_KEY, dbValue);

    }

    private void OnSfxVolumeChange(float value)
    {
        PlayerPrefs.SetFloat(SFX_PREF_KEY, value);
        PlayerPrefs.Save();
        float dbValue = Mathf.Lerp(-30f, 10f, value);
        audioMixer.SetFloat(SFX_PREF_KEY, dbValue);
    }
    private void OnBackButtonClick() => mainMenuHandler.ShowMainMenu();

    private void OnBackGroundOptionsButtonClick() 
    {
        foreach (GameObject obj in ToHideObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        LeftBackGroundButton.gameObject.SetActive(true);
        RightBackGroundButton.gameObject.SetActive(true);
        BackToSettingsButton.gameObject.SetActive(true);

        if (cameraBackGroundHandler.CurrentIndex == 0)
        {
            LeftBackGroundButton.gameObject.SetActive(false);
        }
        if (cameraBackGroundHandler.CurrentIndex == cameraBackGroundHandler.BackGroundsList.Count-1)
        {
            RightBackGroundButton.gameObject.SetActive(false);
        }

        temp_BG_index = cameraBackGroundHandler.CurrentIndex;

    }

    private void OnLeftBackGroundButtonClick()
    {
        temp_BG_index = temp_BG_index - 1;

        cameraBackGroundHandler.CurrentBackGround.sprite = cameraBackGroundHandler.BackGroundsList[temp_BG_index];

        if (temp_BG_index == 0)
        {
            LeftBackGroundButton.gameObject.SetActive(false);
        }

        if (temp_BG_index < cameraBackGroundHandler.BackGroundsList.Count - 1)
        {
            RightBackGroundButton.gameObject.SetActive(true);
        }


        if(temp_BG_index!= cameraBackGroundHandler.CurrentIndex)
        {
            SellectBackGroundButton.gameObject.SetActive(true);

        }
        else
        {
            SellectBackGroundButton.gameObject.SetActive(false);

        }

        BackChangeText?.gameObject.SetActive(false);

    }


    private void OnRightBackGroundButtonClick()
    {
        temp_BG_index = temp_BG_index + 1;


        cameraBackGroundHandler.CurrentBackGround.sprite = cameraBackGroundHandler.BackGroundsList[temp_BG_index];


        if (temp_BG_index > 0)
        {
            LeftBackGroundButton.gameObject.SetActive(true);
        }

        if (temp_BG_index == cameraBackGroundHandler.BackGroundsList.Count - 1)
        {
            RightBackGroundButton.gameObject.SetActive(false);
        }
        if (temp_BG_index != cameraBackGroundHandler.CurrentIndex)
        {
            SellectBackGroundButton.gameObject.SetActive(true);

        }
        else
        {
            SellectBackGroundButton.gameObject.SetActive(false);

        }
        BackChangeText?.gameObject.SetActive(false);


    }

    private void OnBackToSettingsButtonClick()
    {
        foreach (GameObject obj in ToHideObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        LeftBackGroundButton.gameObject.SetActive(false);
        RightBackGroundButton.gameObject.SetActive(false);

      
        BackToSettingsButton.gameObject.SetActive(false);
        SellectBackGroundButton.gameObject.SetActive(false);

        cameraBackGroundHandler.CurrentBackGround.sprite = cameraBackGroundHandler.BackGroundsList[cameraBackGroundHandler.CurrentIndex];
        BackChangeText?.gameObject.SetActive(false);

    }


    private void OnSellectBackGroundButtonClick()
    {


        cameraBackGroundHandler.CurrentIndex = temp_BG_index;

        SellectBackGroundButton.gameObject.SetActive(false) ;

        DataHandler.Instance.backGroundIndex = temp_BG_index;

        DataHandler.Instance.SetBackGroundIndex(cameraBackGroundHandler.CurrentIndex);
        Debug.Log("done");
        backchangetoast(1500);
    }

    private async void backchangetoast(int milis)
    {
        if (BackChangeText == null)
            return;

        var group = BackChangeText.GetComponent<CanvasGroup>();
        if (group == null)
            group = BackChangeText.gameObject.AddComponent<CanvasGroup>();

        BackChangeText.gameObject.SetActive(true);

        // Start fully visible
        group.alpha = 1f;

        // Wait for visible duration (1 second full alpha)
        await Task.Delay(1500);

        // Then gradually fade out
        float fadeDuration = 0.5f; // seconds
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            await Task.Yield();
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
        }

        group.alpha = 0f;
        BackChangeText.gameObject.SetActive(false);
    }


}
