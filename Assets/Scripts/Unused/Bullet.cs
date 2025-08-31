using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDestroyable destroy))
        {
            destroy.OnCollided(); // Call the OnCollided method of the IDestroyable interface
            Destroy(gameObject); // Destroy the bullet after collision
        }
    }
}
