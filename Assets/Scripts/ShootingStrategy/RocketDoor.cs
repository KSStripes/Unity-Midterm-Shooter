using UnityEngine;

public class RocketDoor : MonoBehaviour, IDestroyable
{
    [Header("Explosion Settings")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float destroyDelay = 4f; // Delay before final destruction

    [Header("Material Settings")]
    [SerializeField] private Material debrisMat; // Material to apply to door debris;

    private Renderer[] childRenderers;

    private void Awake()
    {
        // Get Renderer components in child doors
        childRenderers = GetComponentsInChildren<Renderer>();
    }

    public void OnCollided()
    {
        // Spawn explosion effect immediately
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 5f);
        }

        // Change material of child doors to debris material
        if (debrisMat != null)
        {
            foreach (Renderer rend in childRenderers)
            {
                rend.material = debrisMat;
            }
        }

        // Destroy the door after a delay
        Destroy(gameObject, destroyDelay);
    }
}
