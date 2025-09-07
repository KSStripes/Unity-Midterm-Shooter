using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour
{
    public UnityEvent OnTargetHit;

    public void HitTarget()
    {
        OnTargetHit?.Invoke();
    }
}
