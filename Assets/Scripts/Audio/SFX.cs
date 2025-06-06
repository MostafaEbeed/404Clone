using UnityEngine;
using System.Collections; // Needed for IEnumerator

[RequireComponent(typeof(AudioSource))]
public class SFX : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // Ensure it doesn't play automatically if somehow checked on prefab
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// Plays the assigned audio clip and destroys the GameObject afterwards.
    /// </summary>
    /// <param name="clipToPlay">The AudioClip to play.</param>
    /// <param name="mixerGroup">The mixer group to route the sound through.</param>
    public void Play(AudioClip clipToPlay, UnityEngine.Audio.AudioMixerGroup mixerGroup)
    {
        if (clipToPlay == null || audioSource == null)
        {
            Debug.LogWarning("SFXPlayer: Missing clip or AudioSource.", this);
            Destroy(gameObject); // Destroy immediately if setup is wrong
            return;
        }

        audioSource.clip = clipToPlay;
        audioSource.outputAudioMixerGroup = mixerGroup; // Route to the correct group (SFX)
        audioSource.Play();

        // Start coroutine to destroy after clip finishes
        StartCoroutine(DestroyAfterPlaying(clipToPlay.length));
    }

    private IEnumerator DestroyAfterPlaying(float delay)
    {
        // Wait for the duration of the audio clip
        yield return new WaitForSeconds(delay);

        // Destroy this GameObject
        Destroy(gameObject);
    }
}