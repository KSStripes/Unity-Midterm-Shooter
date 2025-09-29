using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [Header("Music Clips")]
    [SerializeField] private AudioClip level0Music;
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

        // keep music across scene loads if you want
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        AudioClip clip = null;
        switch (state)
        {
            case GameState.Level_0: clip = level0Music; break;
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
        }
    }
}
