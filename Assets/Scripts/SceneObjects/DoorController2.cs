// DoorController.cs — requires 2 pads; opens door + handles NavMeshObstacle carving.

using UnityEngine;
using UnityEngine.AI;

public class DoorController2 : MonoBehaviour
{
    [Header("Door visuals")]
    [SerializeField] private Animator doorAnimator;                  // Animator (bool DoorOpen)

    [Header("NavMesh blocking")]
    [SerializeField] private string doorOpenParam = "DoorOpen";      // animator bool name
    [SerializeField] private bool doorStartsOpen = false;            // optional initial state
    private NavMeshObstacle navObstacle;                             // auto-found

    // pad states
    private bool pad1Active;
    private bool pad2Active;

    void Awake()
    {
        // auto-find obstacle on self/children and configure carving
        navObstacle = GetComponentInChildren<NavMeshObstacle>(true);
        if (navObstacle)
        {
            navObstacle.carving = true;
            navObstacle.carveOnlyStationary = true;
        }

        // initialize from animator or fallback flag
        bool startOpen = doorAnimator ? doorAnimator.GetBool(doorOpenParam) : doorStartsOpen;
        ApplyDoorState(startOpen);
    }

    /// <summary>
    /// Called by PressurePad(s). padIndex should be 1 or 2.
    /// </summary>
    public void SetPadActive(int padIndex, bool active)
    {
        if (padIndex == 1) pad1Active = active;
        else if (padIndex == 2) pad2Active = active;

        // open only when both pads are active
        bool shouldOpen = pad1Active && pad2Active;
        ApplyDoorState(shouldOpen);
    }

    // ----- internals -----

    private void ApplyDoorState(bool open)
    {
        // animator drive
        if (doorAnimator) doorAnimator.SetBool(doorOpenParam, open);

        // navmesh carve toggle: closed => enabled (blocks), open => disabled
        if (navObstacle) navObstacle.enabled = !open;
    }
}
