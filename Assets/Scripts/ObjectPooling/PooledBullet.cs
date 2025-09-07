using UnityEngine;

public class PooledBullet : PooledObject
{

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the IDestroyable component
        if (collision.gameObject.TryGetComponent(out IDestroyable destroy))
        {
            destroy.OnCollided(); //destroy the collided bullet
            linkedPool.ResetBullet(this); //return bullet to pool
        }

        // Check if the collided object is the target
        else if (collision.gameObject.TryGetComponent(out Target target))
        {
            target.HitTarget(); // Notify the target that it has been hit
            linkedPool.ResetBullet(this); // Return bullet to pool
        }
    }
}
