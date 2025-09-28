using UnityEngine;
using UnityEngine.AI; // for NavMeshObstacle

public class DoorPressurePad : MonoBehaviour
{
    [Header("Pad detection")]
    [SerializeField] private float detectionRadius = 0.5f;
    [SerializeField] private LayerMask cubeLayer;

    [Header("Door Settings")]
    [SerializeField] private Animator doorAnimator;      // Animator on the door (bool "DoorOpen")
    [SerializeField] private string doorOpenParam = "DoorOpen";
    [SerializeField] private MeshRenderer doorLight;
    [SerializeField] private Material doorOnMat;
    [SerializeField] private Material doorOffMat;

    [Header("NavMesh blocking")]
    private NavMeshObstacle obstacle; // auto-found
    private readonly Collider[] results = new Collider[10];  // reuse buffer to avoid GC

    private bool cubeOnPad = false;

    void Awake()
    {
        // Find an existing NavMeshObstacle on the door or its children (no auto-create)
        if (doorAnimator)
            obstacle = doorAnimator.GetComponentInChildren<NavMeshObstacle>(true);

        if (obstacle)
        {
            obstacle.carving = true; // ensure carving so it cuts the NavMesh when enabled
            // Initialize obstacle from current door state: closed => block (enabled), open => allow (disabled)
            bool open = doorAnimator ? doorAnimator.GetBool(doorOpenParam) : false;
            ToggleObstacle(block: !open);
        }
    }

    void Update()
    {
        // Check for a cube within the detection radius on the given layer
        System.Array.Clear(results, 0, results.Length);
        Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, results, cubeLayer);
        bool foundCube = false;
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] != null) { foundCube = true; break; }
        }

        if (foundCube && !cubeOnPad)
        {
            cubeOnPad = true;
            CubePlaced();
        }
        else if (!foundCube && cubeOnPad)
        {
            cubeOnPad = false;
            CubeRemoved();
        }
    }

    private void CubePlaced()
    {
        // Visuals on + open door
        if (doorLight && doorOnMat) doorLight.material = doorOnMat;
        if (doorAnimator) doorAnimator.SetBool(doorOpenParam, true);

        // Let AI pass through (disable obstacle)
        ToggleObstacle(block: false);
    }

    private void CubeRemoved()
    {
        // Visuals off + close door
        if (doorLight && doorOffMat) doorLight.material = doorOffMat;
        if (doorAnimator) doorAnimator.SetBool(doorOpenParam, false);

        // Block AI again (enable obstacle)
        ToggleObstacle(block: true);
    }

    private void ToggleObstacle(bool block)
    {
        if (!obstacle) return;
        obstacle.enabled = block; // enabled => blocks (carves), disabled => allows passage
        // carving property already set in Awake; Unity 6 exposes it as 'carving'
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
