using UnityEngine;

// Handles background music switching based on game state
// Handles background music switching based on game state
[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f; // Duration for fade in/out
    private Coroutine fadeCoroutine;
    [Header("Music Clips")]
    [SerializeField] private AudioClip level1Music;
    [SerializeField] private AudioClip level2Music;
    [SerializeField] private AudioClip level3Music;
    [SerializeField] private AudioClip level4Music;
    [SerializeField] private AudioClip level5Music;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip gameWinMusic;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = false;
        source.mute = false;      // ensure not muted

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
            // start with current state music
            OnGameStateChanged(GameManager.Instance.CurrentGameState);
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
        }
    }

    // Switch music based on game state
    public void OnGameStateChanged(GameState state)
    {
        AudioClip clip = null;
        switch (state)
        {
            case GameState.Level_1: clip = level1Music; break;
            case GameState.Level_2: clip = level2Music; break;
            case GameState.Level_3: clip = level3Music; break;
            case GameState.Level_4: clip = level4Music; break;
            case GameState.Level_5: clip = level5Music; break;
            case GameState.GameOver: clip = gameOverMusic; break;
            case GameState.GameWin: clip = gameWinMusic; break;
        }

        if (clip != null && source.clip != clip)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOutIn(clip));
            Debug.Log($"[MusicManager] Now playing: {clip.name}");
        }
    }

    // Coroutine to fade out current track, switch, then fade in new track
    private System.Collections.IEnumerator FadeOutIn(AudioClip newClip)
    {
        float startVolume = source.volume;
        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
        source.clip = newClip;
        source.Play();
        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            source.volume = Mathf.Lerp(0f, startVolume, t / fadeDuration);
            yield return null;
        }
        source.volume = startVolume;
    }
}