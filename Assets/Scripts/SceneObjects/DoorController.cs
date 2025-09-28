using UnityEngine;
using UnityEngine.AI; // for NavMeshObstacle

public class DoorController : MonoBehaviour
{
    [Header("Door visuals")]
    [SerializeField] private Animator doorAnimator;         // Animator with bool "DoorOpen"
    [SerializeField] private MeshRenderer doorLight;
    [SerializeField] private MeshRenderer stripe1;
    [SerializeField] private MeshRenderer stripe2;
    [SerializeField] private MeshRenderer stripe3;
    [SerializeField] private MeshRenderer stripe4;

    [SerializeField] private Material doorOnMat;            // green
    [SerializeField] private Material doorOffMat;           // red

    [Header("Open logic")]
    [SerializeField] private int doorWaitTime = 2;          // seconds player must stay in trigger
    private float currentDoorWaitTime;
    private bool playerInside;

    [Header("NavMesh blocking (no setup needed)")]
    [SerializeField] private string doorOpenParam = "DoorOpen"; // animator bool name
    private NavMeshObstacle navObstacle; // auto-found
    [SerializeField] private bool doorStartsOpen = false;         // set to true if scene starts open

    void Awake()
    {
        // Auto-find an obstacle on this object or child objects
        navObstacle = GetComponentInChildren<NavMeshObstacle>(true);
        if (navObstacle)
        {
            navObstacle.carving = true;
            navObstacle.carveOnlyStationary = true;

            // Initialize obstacle state from starting door state
            bool open = doorAnimator ? doorAnimator.GetBool(doorOpenParam) : doorStartsOpen;
            navObstacle.enabled = !open; // closed => enabled (blocks), open => disabled
        }
    }

    // ---- Trigger handling ----
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        SetLightMaterial(doorOnMat);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SetDoorOpen(false);           // start closing
        SetLightMaterial(doorOffMat); // lights red
        playerInside = false;
        currentDoorWaitTime = 0f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void Update()
    {
        if (currentDoorWaitTime >= doorWaitTime)
        {
            SetDoorOpen(true); // open once the wait time is reached
            return;
        }

        if (playerInside)
            currentDoorWaitTime += Time.deltaTime;
    }

    // ---- Helpers ----
    private void SetLightMaterial(Material material)
    {
        if (doorLight) doorLight.material = material;
        if (stripe1)   stripe1.material = material;
        if (stripe2)   stripe2.material = material;
        if (stripe3)   stripe3.material = material;
        if (stripe4)   stripe4.material = material;
    }

    private void SetDoorOpen(bool open)
    {
        // Drive the animator
        if (doorAnimator) doorAnimator.SetBool(doorOpenParam, open);

        // Toggle NavMesh blocking (open => allow path, closed => block path)
        if (navObstacle)
            navObstacle.enabled = !open;
    }
}
