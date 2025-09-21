using UnityEngine;
using UnityEngine.Events;

public class TargetSequenceController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private SequenceTarget[] targets;   // assign targets in Inspector
    private int expectedIndex = 0; // next required sequence index

    [Header("Events (optional)")]
    public UnityEvent onSequenceCompleted;           // fires when last target is hit in order
    public UnityEvent<int> onProgressAdvanced;       // passes number of correct hits so far (1..N)
    public UnityEvent<SequenceTarget> onWrongHit;    // passes the wrong target

    // [Header("Door Settings")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private MeshRenderer doorLight;
    [SerializeField] private Material doorOnMat;

    private void Awake()
    {
        // If not wired in the Inspector, grab children automatically
        if (targets == null || targets.Length == 0)
            targets = GetComponentsInChildren<SequenceTarget>(includeInactive: true);

        if (targets == null || targets.Length == 0)
        {
            Debug.LogWarning($"[SeqController] No targets found under '{name}'", this);
            return;
        }

        // Initialize and sort by SequenceIndex to enforce the intended order
        foreach (var t in targets) t.Initialize(this);
        System.Array.Sort(targets, (a, b) => a.SequenceIndex.CompareTo(b.SequenceIndex));

        // Start with everything OFF
        SetAllIndicators(false);
        expectedIndex = 0;
    }

    // Called by SequenceTarget.RegisterHit()
    public void OnTargetHit(SequenceTarget target)
    {
        // Guard: if we have no targets or already completed
        if (targets == null || targets.Length == 0) return;
        if (expectedIndex >= targets.Length) return; // already complete; ignore

        // Assign the next target as correct target
        var correctTarget = targets[expectedIndex];

        // If the hit target is the expected one
        if (target == correctTarget)
        {
            correctTarget.SetIndicator(true); // Turn on current indicator
            Debug.Log($"[SeqController] Correct target hit: '{target.name}' ({expectedIndex + 1}/{targets.Length}).", this);
            expectedIndex++; // Advance to next expected index

            // Progress callback (1-based progress for UX)
            onProgressAdvanced?.Invoke(expectedIndex);

            if (expectedIndex >= targets.Length)
            {
                // Completed!
                Debug.Log($"[SeqController] Sequence completed ({targets.Length}/{targets.Length}).", this);
                SetAllIndicators(true);
                onSequenceCompleted?.Invoke();
                OpenDoor(); // Open the door when sequence is completed
                return;
            }
        }
        else
        {
            // Wrong target hit
            Debug.Log($"[SeqController] Wrong target hit: '{target.name}'. Expected '{correctTarget.name}'.", this);
            onWrongHit?.Invoke(target);
            // Reset sequence
            ResetToStart();
        }
    }

    // Resets the sequence to the start (all panels off, expected index = 0)
    public void ResetToStart()
    {
        SetAllIndicators(false);
        expectedIndex = 0;
        Debug.Log("[SeqController] Sequence reset (all panels OFF).", this);
    }

    // Helper to set all target panels on or off
    private void SetAllIndicators(bool on)
    {
        foreach (var t in targets) t.SetIndicator(on);
    }

    // Door opening logic
    private void OpenDoor()
    {
        // Play animation if Animator is assigned
        if (doorAnimator)
        {
            doorAnimator.SetBool("DoorOpen", true);
        }

        // Change light material if assigned
        if (doorLight && doorOnMat)
        {
            doorLight.material = doorOnMat;
        }

        Debug.Log("[SeqController] Door opened and light changed.", this);
    }
}