using System;
using UnityEngine;

// Singleton GameManager to track and manage game state
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;// Singleton instance
    [SerializeField] private GameState currentGameState = GameState.Briefing; // Initialize GameState in Inspector
    //public GameState currentGameState { get; private set; }
    public Action<GameState> OnStateChanged; // Event listener for state changes
    public GameState CurrentGameState => currentGameState; // Public getter

    private void Awake()
    {
        // Ensure only one GameManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate
        }
    }

    // Change the current game state
    public void ChangeState(GameState state)
    {
        if (currentGameState != state)
        {
            currentGameState = state;
            OnStateChanged?.Invoke(currentGameState); // Notify event listeners
            Debug.Log("Game State changed to: " + currentGameState);
        }
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
