using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class BGMHandler : MonoBehaviour
{
    public static BGMHandler Instance { get; private set; } // Singleton instance

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float fadeDuration = 1f; // Duration of the fade effect
    public float BGMVolume = 1f;

    // Scene-to-track mapping
    [System.Serializable]
    public struct SceneMusic
    {
        public string sceneName; // Name of the scene
        public AudioClip musicTrack; // Music track for the scene
    }

    [SerializeField] private List<SceneMusic> sceneMusicList; // List of scene-music pairs

    private string currentSceneName; // Name of the current scene

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    private void Start()
    {
        // Play music for the initial scene
        currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Audio :" + currentSceneName);
        PlayTrackForScene(currentSceneName);

        // Subscribe to scene load events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene load events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Called when a new scene is loaded.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = scene.name;

        // Check if the scene has changed
        if (newSceneName != currentSceneName)
        {
            currentSceneName = newSceneName;
            PlayTrackForScene(currentSceneName);
        }
    }

    /// <summary>
    /// Play the music track for the specified scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene.</param>
    private void PlayTrackForScene(string sceneName)
    {
        // Find the music track for the scene
        AudioClip track = GetTrackForScene(sceneName);

        if (track != null)
        {
            // Fade out the current track
            FadeOut(() =>
            {
                // Switch to the new track
                // audioSource.clip = track;
                // audioSource.loop = true;
                // audioSource.volume = BGMVolume;
                // audioSource.Play();
                PlayMusicClip(track, transform, BGMVolume);
                // Fade in the new track
                FadeIn();
            });
        }
        else
        {
            Debug.LogWarning($"No music track found for scene: {sceneName}");
        }
    }

    /// <summary>
    /// Get the music track for the specified scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene.</param>
    /// <returns>The music track for the scene, or null if not found.</returns>
    private AudioClip GetTrackForScene(string sceneName)
    {
        foreach (var sceneMusic in sceneMusicList)
        {
            if (sceneMusic.sceneName == sceneName)
            {
                return sceneMusic.musicTrack;
            }
        }
        return null;
    }

    /// <summary>
    /// Fade out the current track.
    /// </summary>
    /// <param name="onFadeComplete">Callback when the fade is complete.</param>
    private void FadeOut(System.Action onFadeComplete = null)
    {
        StartCoroutine(FadeVolume(0f, onFadeComplete));
    }

    /// <summary>
    /// Fade in the current track.
    /// </summary>
    private void FadeIn()
    {
        StartCoroutine(FadeVolume(1f));
    }

    private IEnumerator FadeVolume(float targetVolume, System.Action onFadeComplete = null)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
        onFadeComplete?.Invoke();
    }

    public void PlayMusicClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(this.audioSource, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.Play();

        float length = audioSource.clip.length;
        Destroy(audioSource.gameObject, length);
    }

    /// <summary>
    /// Stop the music.
    /// </summary>
    public void Stop()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); // Stop playing
        }
    }

    /// <summary>
    /// Pause the music.
    /// </summary>
    public void Pause()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause(); // Pause playing
        }
    }

    /// <summary>
    /// Resume the music.
    /// </summary>
    public void Resume()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause(); // Resume playing
        }
    }
}
