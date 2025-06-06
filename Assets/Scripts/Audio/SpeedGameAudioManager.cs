using UnityEngine;
using UnityEngine.Audio;
using System.Runtime.InteropServices;

public class SpeedGameAudioManager : MonoBehaviour
{
    public static SpeedGameAudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup sfxNoEffectMixerGroup;

    [Header("Audio Sources")]
    [Tooltip("AudioSource dedicated to playing music.")]
    [SerializeField] private AudioSource musicSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameOverMusic;

    [Header("SFX Settings")]
    [Tooltip("Prefab with AudioSource and SFXPlayer script.")]
    [SerializeField] private GameObject sfxPrefab;
    
    [DllImport("__Internal")]
    private static extern void RegisterVisibilityChangeCallback();

    private const string MusicVolumeParam = "MusicVolume";
    private const string SfxVolumeParam = "SFXVolume";
    
    private AudioClip currentMusicClip;

    public enum MusicType { Gameplay, MainMenu, GameOver }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        #if UNITY_WEBGL && !UNITY_EDITOR
        RegisterVisibilityChangeCallback();
        #endif
        
        ValidateSetup();
        InitializeMusicSource();
    }

    private void Start()
    {
        PlayMusic(MusicType.MainMenu);
    }
    
    public void OnVisibilityChange(string state)
    {
        if (state == "hidden")
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
        else if (state == "visible")
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }

    private void ValidateSetup()
    {
        if (mainMixer == null) 
            Debug.LogError("AudioManager: Main Mixer not assigned!", this);
        if (musicMixerGroup == null) 
            Debug.LogError("AudioManager: Music Mixer Group not assigned!", this);
        if (sfxMixerGroup == null) 
            Debug.LogError("AudioManager: SFX Mixer Group not assigned!", this);
        if (musicSource == null)
            Debug.LogError("AudioManager: Music Source not assigned!", this);
        if (sfxPrefab == null) 
            Debug.LogError("AudioManager: SFX Prefab not assigned!", this);
        else if (sfxPrefab.GetComponent<SFX>() == null)
            Debug.LogError("AudioManager: SFX Prefab is missing the SFX script!", sfxPrefab);
    }

    private void InitializeMusicSource()
    {
        if (musicSource != null)
        {
            musicSource.outputAudioMixerGroup = musicMixerGroup;
            musicSource.loop = true;
        }
    }

    public void PlayMusic(MusicType type)
    {
        if (musicSource == null) return;

        AudioClip clipToPlay = GetMusicClip(type);
        
        if (clipToPlay != null)
        {
            if (currentMusicClip == clipToPlay && musicSource.isPlaying)
                return;

            currentMusicClip = clipToPlay;
            musicSource.clip = clipToPlay;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"AudioManager: Music clip for type {type} is not assigned.");
            StopMusic();
        }
    }

    private AudioClip GetMusicClip(MusicType type)
    {
        return type switch
        {
            MusicType.Gameplay => gameplayMusic,
            MusicType.MainMenu => mainMenuMusic,
            MusicType.GameOver => gameOverMusic,
            _ => null
        };
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            currentMusicClip = null;
        }
    }

    public void PlaySFX(AudioClip clip, Vector3 position)
    {
        PlaySFXInternal(clip, position, sfxMixerGroup);
    }

    public void PlaySFXNoEffect(AudioClip clip, Vector3 position)
    {
        PlaySFXInternal(clip, position, sfxNoEffectMixerGroup);
    }

    public void PlaySFX(AudioClip clip)
    {
        PlaySFX(clip, transform.position);
    }

    public void PlaySFXNoEffect(AudioClip clip)
    {
        PlaySFXNoEffect(clip, transform.position);
    }

    private void PlaySFXInternal(AudioClip clip, Vector3 position, AudioMixerGroup mixerGroup)
    {
        if (clip == null || sfxPrefab == null || mixerGroup == null)
        {
            Debug.LogWarning("AudioManager: Cannot play SFX - Missing clip, prefab, or mixer group.");
            return;
        }

        GameObject sfxInstance = Instantiate(sfxPrefab, position, Quaternion.identity);
        SFX player = sfxInstance.GetComponent<SFX>();
        
        if (player != null)
        {
            player.Play(clip, mixerGroup);
        }
        else
        {
            Debug.LogError("AudioManager: SFX Prefab instance missing SFX script!", sfxInstance);
            Destroy(sfxInstance);
        }
    }

    public void SetMusicVolume(float volume)
    {
        SetMixerVolume(MusicVolumeParam, volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetMixerVolume(SfxVolumeParam, volume);
    }

    private void SetMixerVolume(string parameterName, float volume)
    {
        if (mainMixer == null) return;
        
        float clampedVolume = Mathf.Clamp(volume, 0.0001f, 1.0f);
        float dBVolume = Mathf.Log10(clampedVolume) * 20f;
        mainMixer.SetFloat(parameterName, dBVolume);
    }
}