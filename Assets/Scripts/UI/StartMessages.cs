using UnityEngine;

public class StartMessages : MonoBehaviour
{
    [SerializeField] private HUDMessageUI ui; // Ref to UI MessagePanel
    [SerializeField] private MessageItem[] messages;   // Write messages in inspector
    [SerializeField] private bool playOnStart = true;  // Tick to auto-play on scene start

    private void Start()
    {
        if (playOnStart) Play();
    }

    // Call this from a UnityEvent/Buttons if you want to trigger manually
    public void Play()
    {
        if (!ui)
        {
            Debug.LogWarning("StartMessages: HUDMessageUI reference not set.");
            return;
        }
        if (messages == null || messages.Length == 0) return;

        ui.ShowSequenceAuto(this, messages); // auto-show each line then fade
    }
}
