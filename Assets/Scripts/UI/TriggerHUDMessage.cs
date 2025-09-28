using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TriggerHUDMessage : MonoBehaviour
{
    [SerializeField] private HUDMessageUI ui; // Ref to UI MessagePanel
    [SerializeField] private MessageItem[] messages;  // Assign in Inspector
    [SerializeField] private bool onceOnly = true;

    private bool used;

    private void Reset()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onceOnly && used) return; // stop if already displayed
        if (!other.CompareTag("Player")) return;
        if (!ui || messages == null || messages.Length == 0) return;

        used = true;
        ui.ShowSequenceAuto(messages); // shows each line
    }
}
