using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip musicClip;   // Assign in Inspector
    private static MusicPlayer instance;
    private AudioSource audioSource;

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

            // Set volume from PlayerPrefs or use default
            audioSource.volume = PlayerPrefs.GetFloat("music_volume", 0.5f);

            audioSource.Play();
        }
        else
        {
            // Prevent duplicates on scene reload
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
