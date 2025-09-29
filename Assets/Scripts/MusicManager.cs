using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
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
        source.spatialBlend = 0f; // 2D sound
        source.volume = 0.7f;       // full volume
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
            source.clip = clip;
            source.Play();
            Debug.Log($"[MusicManager] Now playing: {clip.name}");
        }
    }
}
