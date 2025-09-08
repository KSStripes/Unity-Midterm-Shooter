using UnityEngine;

// Implements IShootStrategy for rocket firing, not a MonoBehaviour
public class RocketShootStrategy : IShootStrategy
{
    // Reference to the weapon's signifier, color changing box
    private GameObject weaponSignifier;

    // Constructor: sets the signifier and changes its color to yellow
    public RocketShootStrategy(GameObject signifier)
    {
        weaponSignifier = signifier;
        //Debug.Log("Rocket Strategy Selected");
        weaponSignifier.GetComponent<Renderer>().material.color = Color.red;
    }


    // Shoot method: retrieves a rocket from the pool, positions it, and applies force
    public void Shoot(ShootAbility ability)
    {
        Rigidbody clonedProjectile = ability.rocketPool.RetrieveAvailableBullet().GetRigidbody();

        if (clonedProjectile == null)
        {
            Debug.LogError("No objects available in the object pool.");
            return;
        }

        // Set rocket position and rotation to firePoint
        clonedProjectile.position = ability.firePoint.position;
        clonedProjectile.rotation = ability.firePoint.rotation;
        // Apply force to shoot the rocket
        clonedProjectile.AddForce(ability.firePoint.forward * ability.GetShootForce());
    }
}
