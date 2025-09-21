using UnityEngine;

[DisallowMultipleComponent]
public class SequenceTarget : MonoBehaviour
{
    [SerializeField] private int sequenceIndex = 0;
    public int SequenceIndex => sequenceIndex;

    [SerializeField] private GameObject panelObject;  // assign in Inspector

    private TargetSequenceController controller;


    // called by controller during Awake()
    public void Initialize(TargetSequenceController controller)
    {
        this.controller = controller; //  Assign controller
        
        // Auto-find the panel if not assigned
        if (!panelObject)
        {
            panelObject = transform.Find("Panel")?.gameObject;
        }

        SetIndicator(false); // start with panel off
    }

    // called by PooledBullet when hit
    public void RegisterHit()
    {
        controller?.OnTargetHit(this);  // notify controller
    }

    // initialized indicator panel as false and activate by controller
    public void SetIndicator(bool on)
    {
        if (!panelObject)
        {
            Debug.LogWarning($"[SeqTarget #{sequenceIndex}] No panel assigned on '{name}'", this);
            return;
        }
        panelObject.SetActive(on);
    }
}
