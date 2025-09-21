using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class SequenceTarget : MonoBehaviour
{
    [SerializeField] private int sequenceIndex = 0;
    public int SequenceIndex => sequenceIndex;

    [SerializeField] private GameObject indicatorObject;

    public UnityEvent OnTargetHit;

    private TargetSequenceController controller;

    public void Init(TargetSequenceController owner)
    {
        controller = owner;
        Debug.Log($"[SeqTarget #{sequenceIndex}] Init on '{name}'. Indicator={(indicatorObject ? indicatorObject.name : "NULL")}", this);
        SetIndicator(false);
    }

    // called by PooledBullet when hit
    public void RegisterHit()
    {
        HitTarget();
    }

    public void HitTarget()
    {
        controller?.OnTargetHit(this);  // notify controller
        OnTargetHit?.Invoke(); // make green light active
    }

    // called by controller
    public void SetIndicator(bool on)
    {
        if (indicatorObject)
        {
            indicatorObject.SetActive(on);
        }
        else
        {
            Debug.LogWarning($"[SeqTarget #{sequenceIndex}] No indicator assigned on '{name}'", this);
        }
    }
}
