using UnityEngine;

// Implements IShootStrategy for bullet firing, not a MonoBehaviour
public class BulletShootStrategy : IShootStrategy
{
    // Reference to weapon's visual signifier (color changing box)
    private GameObject weaponSignifier;

    // Constructor: sets the signifier and changes its color to red
    public BulletShootStrategy(GameObject signifier)
    {
        weaponSignifier = signifier;
        //Debug.Log("Bullet Strategy Selected");
        weaponSignifier.GetComponent<Renderer>().material.color = Color.cyan;
    }


    // Shoot method: retrieves a bullet from the pool, positions it, and applies force
    public void Shoot(ShootAbility ability)
    {
        Rigidbody clonedProjectile = ability.bulletPool.RetrieveAvailableBullet().GetRigidbody();

        if (clonedProjectile == null)
        {
            Debug.LogError("No objects available in the object pool.");
            return;
        }

        // Set bullet position and rotation to firePoint
        clonedProjectile.position = ability.firePoint.position;
        clonedProjectile.rotation = ability.firePoint.rotation;
        // Apply force to shoot the bullet
        clonedProjectile.AddForce(ability.firePoint.forward * ability.GetShootForce());
    }
}
