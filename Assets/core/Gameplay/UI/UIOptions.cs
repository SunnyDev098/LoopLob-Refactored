using UnityEngine;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button BackButton;
    [SerializeField] private MainMenuHandler mainMenuHandler;

    private const string MUSIC_PREF_KEY = "music_volume";
    private const string SFX_PREF_KEY = "sfx_volume";

    private void Awake()
    {
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_PREF_KEY, 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_PREF_KEY, 0.5f);

        musicSlider.onValueChanged.AddListener(OnMusicVolumeChange);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChange);
        BackButton.onClick.AddListener(OnBackButtonClick);
    }

    private void OnMusicVolumeChange(float value)
    {
        PlayerPrefs.SetFloat(MUSIC_PREF_KEY, value);
        PlayerPrefs.Save();
        MusicPlayer.SetVolume(value);
    }

    private void OnSfxVolumeChange(float value)
    {
        PlayerPrefs.SetFloat(SFX_PREF_KEY, value);
        PlayerPrefs.Save();
    }
    private void OnBackButtonClick() => mainMenuHandler.ShowMainMenu();
   
}
