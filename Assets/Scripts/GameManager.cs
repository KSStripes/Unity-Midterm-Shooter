using System;
using System.Collections.Generic;
using UnityEngine;

// Singleton GameManager to track and manage game state
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;// Singleton instance
    [SerializeField] private GameState currentGameState = GameState.Briefing; // Initialize GameState in Inspector
    [SerializeField] private GameObject winCinematic; // Assign GameWin cinematic
    public Action<GameState> OnStateChanged; // Event listener for state changes
    public GameState CurrentGameState => currentGameState; // Public getter

    [Header("Respawn Settings")]
    public Transform lvl1Res;
    public Transform lvl2Res;
    public Transform lvl3Res;
    public Transform lvl4Res;
    public Transform lvl5Res;


    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Debug.LogWarning("[GameManager] Duplicate destroyed", this); Destroy(gameObject); return; }
    }



    // Change the current game state
    public void ChangeState(GameState state)
    {
        if (currentGameState == state) return;

        currentGameState = state;
        OnStateChanged?.Invoke(currentGameState);

        // Log state change
        var printText = GetStateMessage(currentGameState);
        Debug.Log($"[{nameof(GameManager)}] {printText} (State = {currentGameState})", this);

        // When GameWin, hook up cinematics
        if (currentGameState == GameState.GameWin && winCinematic != null)
        {
            winCinematic.SetActive(true);

            var director = winCinematic.GetComponent<UnityEngine.Playables.PlayableDirector>();
            if (director != null)
            {
                director.time = 0;
                director.Play();
            }
        }
    }

    // Map states to readable messages, also used in UIController for HUD display
    private static readonly Dictionary<GameState, string> StateMessages = new()
    {
        { GameState.Briefing, "Briefing started." },
        { GameState.Level_1, "You have reached Level 1." },
        { GameState.Level_2, "You have reached Level 2." },
        { GameState.Level_3, "You have reached Level 3." },
        { GameState.Level_4, "You have reached Level 4." },
        { GameState.Level_5, "You have reached Level 5." },
        { GameState.GameOver, "You have lost!" },
        { GameState.GameWin, "You have won!" },
    };

    public static string GetStateMessage(GameState state)
        => StateMessages.TryGetValue(state, out var msg) ? msg : $"State changed to {state}.";

    public void FinishIntroCutscene()
    {
        Debug.Log("[GameManager] FinishIntroCutscene() signal RECEIVED", this);
        ChangeState(GameState.Level_1);
    }

    // Respawn player at level location based on current game state and reset health
// Respawn player at level location based on current game state and reset health
    public void RespawnPlayer(Transform player, HealthSystem hp)
    {
        // Reset health/state
        hp.isDead = false;
        hp.IncreaseHealth(hp.GetMaxHealth()); // clamps + fires OnHealthChanged

        // If the player has a CharacterController, disable it while repositioning to avoid unwanted physics snaps
        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        // Temporarily disable player during repositioning (optional if using CC toggle)
        player.gameObject.SetActive(false);

        // Move to the respawn point for the current state
        switch (currentGameState)
        {
            case GameState.Level_1:
                player.position = lvl1Res.position;
                break;
            case GameState.Level_2:
                player.position = lvl2Res.position;
                break;
            case GameState.Level_3:
                player.position = lvl3Res.position;
                break;
            case GameState.Level_4:
                player.position = lvl4Res.position;
                break;
            case GameState.Level_5:
                player.position = lvl5Res.position;
                break;
            default:
                Debug.LogWarning("[GameManager] RespawnPlayer() called in invalid state: " + currentGameState, this);
                break;
        }

        // Re-enable player + controller
        player.gameObject.SetActive(true);
        if (cc) cc.enabled = true;
    }
}

// Enum for all possible game states
public enum GameState
{
    Briefing, // Intro Cutscene
    Level_1,
    Level_2,
    Level_3,
    Level_4,
    Level_5,
    GameOver, // Player dies
    GameWin,  // Player completes final level
}
