using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button BackButton;
    [SerializeField] private MainMenuHandler mainMenuHandler;
    [SerializeField] private AudioMixer  audioMixer;

    private const string MUSIC_PREF_KEY = "MusicVolume";
    private const string SFX_PREF_KEY = "SFXVolume";

    private void Awake()
    {
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_PREF_KEY);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_PREF_KEY);


        musicSlider.onValueChanged.AddListener(OnMusicVolumeChange);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChange);
        BackButton.onClick.AddListener(OnBackButtonClick);

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
   
}
