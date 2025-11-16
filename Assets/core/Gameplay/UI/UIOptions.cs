using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button BackButton;
    [SerializeField] private Button BackGroundButton;

    [SerializeField] private Button LeftBackGroundButton;
    [SerializeField] private Button RightBackGroundButton;

    [SerializeField] private Button BackToSettingsButton;

    [SerializeField] private MainMenuHandler mainMenuHandler;
    [SerializeField] private AudioMixer  audioMixer;

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
        BackGroundButton.onClick.AddListener(OnBackGroundButtonClick);

        LeftBackGroundButton.onClick.AddListener(OnLeftBackGroundButtonClick);
        RightBackGroundButton.onClick.AddListener(OnRightBackGroundButtonClick);

        BackToSettingsButton.onClick.AddListener(OnBackToSettingsButtonClick);


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

    private void OnBackGroundButtonClick() 
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


    }

    private void OnLeftBackGroundButtonClick()
    {
       cameraBackGroundHandler.ChangeBackGround(cameraBackGroundHandler.CurrentIndex - 1);

        if (cameraBackGroundHandler.CurrentIndex == 0)
        {
            LeftBackGroundButton.gameObject.SetActive(false);
        }

        if (cameraBackGroundHandler.CurrentIndex < cameraBackGroundHandler.BackGroundsList.Count - 1)
        {
            RightBackGroundButton.gameObject.SetActive(true);
        }
    }


    private void OnRightBackGroundButtonClick()
    {
        cameraBackGroundHandler.ChangeBackGround(cameraBackGroundHandler.CurrentIndex +1 );



        if (cameraBackGroundHandler.CurrentIndex > 0)
        {
            LeftBackGroundButton.gameObject.SetActive(true);
        }

        if (cameraBackGroundHandler.CurrentIndex == cameraBackGroundHandler.BackGroundsList.Count - 1)
        {
            RightBackGroundButton.gameObject.SetActive(false);
        }

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

        PlayerPrefs.Save();
    }
}
