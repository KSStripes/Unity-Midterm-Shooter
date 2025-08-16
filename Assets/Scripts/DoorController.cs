using UnityEngine;

public class DoorController : MonoBehaviour
{
    //trigger when object with Rigidbody and colliders enters the trigger
    private void OnTriggerEnter(Collider other)
    {

    }

    //trigger when object with Rigidbody and colliders exits the trigger
    private void OnTriggerExit(Collider other)
    {

    }

    // trigger when something is spawned within the trigger
    // called every frame that other object is still inside the trigger
    private void OnTriggerStay(Collider other)
    {
        // Handle door opening and closing logic here
        // For example, check for player input or proximity to the door
    }

}
