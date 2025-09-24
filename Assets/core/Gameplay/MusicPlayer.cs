using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip musicClip;
    private static MusicPlayer instance;
    private AudioSource audioSource;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Use existing AudioSource if present, else add one
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = musicClip;
            audioSource.loop = true;

            // Set volume from PlayerPrefs or use 0.5f by default
            audioSource.volume = PlayerPrefs.GetFloat("music_volume", 0.5f);

            audioSource.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void SetVolume(float volume)
    {
        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.volume = volume;
            PlayerPrefs.SetFloat("music_volume", volume);
        }
    }
}
