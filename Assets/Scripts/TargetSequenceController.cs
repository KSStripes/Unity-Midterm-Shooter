using System;
using UnityEngine;

[DisallowMultipleComponent]
public class TargetSequenceController : MonoBehaviour
{
    [SerializeField] private SequenceTarget[] targets;   // assign in any order; we'll sort
    [Header("Door Settings")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private MeshRenderer doorLight;
    [SerializeField] private Material doorOnMat;
    [SerializeField] private Material doorOffMat;

    private int progress = 0; // expects index 0 next, then 1, ...

    private void Awake()
    {
        if (targets == null || targets.Length == 0)
            targets = GetComponentsInChildren<SequenceTarget>(true);

        // Ensure order 0..N-1 by SequenceIndex
        Array.Sort(targets, (a, b) => a.SequenceIndex.CompareTo(b.SequenceIndex));

        foreach (var t in targets)
            t.Init(this);          // <-- vital: gives each target a controller reference

        ResetSequence();            // start with all indicators off, progress = 0
    }

    public void OnTargetHit(SequenceTarget target)
    {
        // correct next target?
        if (target.SequenceIndex == progress)
        {
            target.SetIndicator(true);
            progress++;

            // finished last one? (e.g., index 4 when there are 5 targets)
            if (progress >= targets.Length)
            {
                doorLight.material = doorOnMat;
                doorAnimator.SetBool("DoorOpen", true);
            }
        }
        else
        {
            // wrong order -> reset everything
            ResetSequence();
        }
    }

    public void ResetSequence()
    {
        progress = 0;
        foreach (var t in targets)
            t.SetIndicator(false);
    }
}
