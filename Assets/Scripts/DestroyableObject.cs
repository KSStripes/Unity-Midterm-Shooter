using UnityEngine;

public class DestroyableObject : MonoBehaviour, IDestroyable
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float destroyDelay = 0.5f; // seconds

    public void OnCollided()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject, destroyDelay);
    }
}
