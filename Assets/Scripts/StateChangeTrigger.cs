using UnityEngine;

public class StateChangeTrigger : MonoBehaviour
{
    public GameState newGameState;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.ChangeState(newGameState);

            // Allow to add commander ability at a later level
            // if (newGameState == GameState.Level_3)
            // {
            //     // Enable commander ability when entering Level 3
            //     PlayerInput.Instance.GainCommander();
            // }

            Destroy(gameObject); // Destroy trigger after use


        }
    }
}
