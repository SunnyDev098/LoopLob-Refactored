using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip musicClip;   // Assign in Inspector
    public static MusicPlayer instance;
    private AudioSource audioSource;

    private const string MUSIC_PREF_KEY = "MusicVolume";
    private const string SFX_PREF_KEY = "SFXVolume";
    [SerializeField] private AudioMixer audioMixer;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // persist across scenes

            // Use existing AudioSource or add one
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = musicClip;
            audioSource.loop = true;


        }
        else
        {
            // Prevent duplicates on scene reload
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        await Task.Delay(300);
        audioSource.Play();

        if (PlayerPrefs.HasKey(MUSIC_PREF_KEY))
        {
            audioMixer.SetFloat(MUSIC_PREF_KEY, Mathf.Lerp(-30f, 10f, PlayerPrefs.GetFloat(MUSIC_PREF_KEY))) ;
        }

        else
        {
            PlayerPrefs.SetFloat(MUSIC_PREF_KEY, 0.5f);
            audioMixer.SetFloat(MUSIC_PREF_KEY, Mathf.Lerp(-30f, 10f, 0.5f));

        }
        if (PlayerPrefs.HasKey(SFX_PREF_KEY))
        {
            audioMixer.SetFloat(SFX_PREF_KEY, Mathf.Lerp(-30f, 10f, PlayerPrefs.GetFloat(SFX_PREF_KEY)));
        }
        else
        {
            PlayerPrefs.SetFloat(SFX_PREF_KEY, 0.5f);
            audioMixer.SetFloat(SFX_PREF_KEY, Mathf.Lerp(-30f, 10f, 0.5f));
        }
    }

    public static void ChangeTrack(AudioClip newClip)
    {
        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.clip = newClip;
            instance.audioSource.Play();
        }
    }

    public static void StopMusic()
    {
        if (instance != null && instance.audioSource.isPlaying)
            instance.audioSource.Stop();
    }

    public static void ResumeMusic()
    {
        if (instance != null && !instance.audioSource.isPlaying)
            instance.audioSource.Play();
    }
}
