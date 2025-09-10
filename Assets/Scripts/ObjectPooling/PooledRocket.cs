using UnityEngine;

public class PooledRocket : PooledObject
{

    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 0.2f;
    [SerializeField] private float explosionForce = 0.2f; // Lower force
    [SerializeField] private float destroyDelay = 0.5f; // Delay before rocket is destroyed

    private void OnCollisionEnter(Collision collision)
    {
        // Spawn explosion effect
        if (explosionPrefab)
        {
            var vfx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 5f);
        }

        var go = collision.collider.gameObject;

        // Allow collider-on-child, script-on-parent setups
        IDestroyable destroy = go.GetComponentInParent<IDestroyable>();
        if (destroy != null)
        {
            destroy.OnCollided();
            linkedPool.ResetBullet(this);   // return rocket to pool 
            return;
        }

        Target target = go.GetComponentInParent<Target>();
        if (target != null)
        {
            target.HitTarget();
            linkedPool.ResetBullet(this);
            return;
        }

        // If it hit something else, still recycle
        linkedPool.ResetBullet(this);
    }
}
