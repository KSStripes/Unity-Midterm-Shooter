using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StateChangeTrigger : MonoBehaviour
{
    // New game state this trigger  sets when the player enters
    public GameState newGameState;

    // Show a HUD message when state changes
    [SerializeField] private UIController uiController;
    [SerializeField] private float messageDuration = 4.0f;

    private void Reset()
    {
        // Ensure collider is set as a trigger (auto on add)
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Get Player only
        if (!other.CompareTag("Player")) return;

        // Update the game state via GameManager
        GameManager.Instance.ChangeState(newGameState);

        // Tell MusicManager to switch tracks
        var musicManager = FindFirstObjectByType<MusicManager>();
        musicManager?.OnGameStateChanged(newGameState);

        // Show state-change message on HUD
        uiController?.ShowStateChangeUI(newGameState, messageDuration);

        // Unlock abilities at specific levels
        if (newGameState == GameState.Level_2)
        {
            PlayerInput.Instance.GetComponent<ShootAbility>()?.UnlockRocket();
        }
        if (newGameState == GameState.Level_4)
        {
            PlayerInput.Instance.GainCommander();
        }

        // Destroy trigger
        Destroy(gameObject);
    }
}
