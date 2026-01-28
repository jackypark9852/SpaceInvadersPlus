using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip gameMusic;
    public AudioClip menuMusic;
    
    [Header("Settings")]
    public float volume = 0.7f;
    public bool loop = true;
    
    static BackgroundMusic instance;
    AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSource();
            PlayGameMusic();
            return;
        }
        
        Destroy(gameObject);
    }

    void SetupAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
    }

    void PlayGameMusic()
    {
        if (gameMusic != null)
        {
            audioSource.clip = gameMusic;
            audioSource.Play();
        }
    }

    public static void PlayMenuMusic()
    {
        if (instance != null && instance.menuMusic != null)
        {
            instance.audioSource.Stop();
            instance.audioSource.clip = instance.menuMusic;
            instance.audioSource.Play();
        }
    }

    public static void PlayGameMusicStatic()
    {
        if (instance != null)
            instance.PlayGameMusic();
    }

    public static void Stop()
    {
        if (instance != null)
            instance.audioSource.Stop();
    }

    public static void SetVolume(float vol)
    {
        if (instance != null)
            instance.audioSource.volume = Mathf.Clamp01(vol);
    }
}