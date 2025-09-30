using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StateChangeTrigger : MonoBehaviour
{
    public GameState newGameState;
    [SerializeField] private UIController uiController;
    [SerializeField] private float messageDuration = 3.0f;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Change the game state
        GameManager.Instance.ChangeState(newGameState);

        // UI message
        uiController?.ShowStateChangeUI(newGameState, messageDuration);

        // Unlock rocket in Room 2
        if (newGameState == GameState.Level_2)
        {
            PlayerInput.Instance.GetComponent<ShootAbility>()?.UnlockRocket();
        }

        // Gain commander in Room 4
        if (newGameState == GameState.Level_4)
        {
            PlayerInput.Instance.GainCommander();
        }

        Destroy(gameObject);
    }
}
