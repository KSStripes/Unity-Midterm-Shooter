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
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 5f); // Destroy explosion effect after 5 seconds
        }

        // Apply force and damage to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
            if (nearbyObject.TryGetComponent(out IDestroyable destroy))
            {
                destroy.OnCollided();
            }
        }

        // Delay destruction so explosion effect is visible
        Destroy(gameObject, destroyDelay);
    }
}
