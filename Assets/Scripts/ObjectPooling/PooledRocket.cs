using System.Collections;
using UnityEngine;

public class PooledRocket : PooledObject
{

    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 0.2f;
    [SerializeField] private float explosionForce = 0.2f; // Lower force
    [SerializeField] private float destroyDelay = 0.5f; // Delay before rocket is destroyed and returned to pool

    private void OnCollisionEnter(Collision collision)
    {
        // Spawn explosion effect
        if (explosionPrefab)
        {
            var vfx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 5f);
        }

        // Apply explosion physics
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        var go = collision.collider.gameObject;

        // Allow collider-on-child, script-on-parent setups
        IDestroyable destroy = go.GetComponentInParent<IDestroyable>();
        if (destroy != null)
        {
            destroy.OnCollided();
            StartCoroutine(WaitAndDestroy()); // Delay before returning to pool
            return;
        }

        Target target = go.GetComponentInParent<Target>();
        if (target != null)
        {
            target.HitTarget();
            StartCoroutine(WaitAndDestroy()); // Delay before returning to pool
            return;
        }

        // If it hit something else, still recycle
        StartCoroutine(WaitAndDestroy()); // Delay before returning to pool
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(destroyDelay);
        linkedPool.ResetBullet(this); // Return rocket to pool
    }
}
