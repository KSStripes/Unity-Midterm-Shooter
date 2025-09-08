using UnityEngine;

public class DestroyableObject : MonoBehaviour, IDestroyable
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float destroyDelay = 0.5f; // seconds

    public void OnCollided()
    {
        // Spawn explosion effect
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 5f); // Destroy explosion effect after 5 seconds
        }
        Destroy(gameObject, destroyDelay);
    }
}
