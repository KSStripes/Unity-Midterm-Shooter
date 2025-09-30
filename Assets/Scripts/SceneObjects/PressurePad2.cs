// PressurePad.cs — detects cube on pad, lights green, notifies DoorController.
// Patterns: SRP (pad only detects & reports), Observer-ish (pad -> controller callback).

using UnityEngine;

public class PressurePad2 : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 0.5f;           // pad radius
    [SerializeField] private LayerMask cubeLayer;                    // layer for  cube or companion
    private readonly Collider[] results = new Collider[8];           // reuse buffer (no GC)

    [Header("Visual")]
    [SerializeField] private MeshRenderer padLightOrDoorLight;       // light to tint (pad or door)
    [SerializeField] private Material onMat;                         // green
    [SerializeField] private Material offMat;                        // red

    [Header("Door Link")]
    [SerializeField] private DoorController2 controller;              // target door
    [SerializeField] private int padIndex = 1;                       // 1 or 2

    private bool cubeOnPad = false;                                  // edge tracking

    void Awake()
    {
        // ensure initial visual
        if (padLightOrDoorLight && offMat) padLightOrDoorLight.material = offMat;
    }

    void Update()
    {
        // sphere check for any cube on the pad
        System.Array.Clear(results, 0, results.Length);
        Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, results, cubeLayer);

        bool found = false;
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] != null) { found = true; break; }
        }

        // rising edge → activate
        if (found && !cubeOnPad)
        {
            cubeOnPad = true;
            if (padLightOrDoorLight && onMat) padLightOrDoorLight.material = onMat; // turn light green
            controller?.SetPadActive(padIndex, true);                               // notify door
        }
        // falling edge → deactivate
        else if (!found && cubeOnPad)
        {
            cubeOnPad = false;
            if (padLightOrDoorLight && offMat) padLightOrDoorLight.material = offMat;
            controller?.SetPadActive(padIndex, false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);  // visualize pad radius
    }
}
