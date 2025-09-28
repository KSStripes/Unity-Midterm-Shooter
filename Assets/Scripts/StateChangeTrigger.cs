using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StateChangeTrigger : MonoBehaviour
{
    public GameState newGameState;
    [SerializeField] private UIController uiController; // Reference to UIController to show messages
    [SerializeField] private float messageDuration = 4.0f; // Duration to show the message
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Change the game state using GameManager
            GameManager.Instance.ChangeState(newGameState);

            //Notify the UIController to show a message
            uiController?.ShowStateChangeUI(newGameState, messageDuration);
            
            // Allow to add rocket shooting commander ability in Room 2
            if (newGameState == GameState.Level_2)
            {
                var shootAbility = PlayerInput.Instance.GetComponent<ShootAbility>();
                shootAbility?.UnlockRocket();
            }

            // Allow to add commander ability in Room 4
            if (newGameState == GameState.Level_4)
            {
                PlayerInput.Instance.GainCommander();
            }

            Destroy(gameObject); // Destroy trigger after use


        }
    }
}
