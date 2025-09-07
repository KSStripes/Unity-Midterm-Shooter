using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator; // Reference to the door's Animator component
    [SerializeField] private MeshRenderer doorLight; // Reference to the door's light MeshRenderer
    [SerializeField] private MeshRenderer stripe1; // Reference to the door's light MeshRenderer
    [SerializeField] private MeshRenderer stripe2; // Reference to the door's light MeshRenderer
    [SerializeField] private MeshRenderer stripe3; // Reference to the door's light MeshRenderer
    [SerializeField] private MeshRenderer stripe4; // Reference to the door's light MeshRenderer

    [SerializeField] private Material doorOnMat; // Material when the light is on
    [SerializeField] private Material doorOffMat; // Material when the light is off
    [SerializeField] private int doorWaitTime = 2; // Time to wait before opening the door
    private float currentDoorWaitTime; // time player is already inside the trigger
    private bool playerInside;


    //trigger when object with Rigidbody and colliders enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // turn all lights green
            doorLight.material = doorOnMat;
            stripe1.material = doorOnMat; 
            stripe2.material = doorOnMat; 
            stripe3.material = doorOnMat;
            stripe4.material = doorOnMat; 
        }
    }

    //trigger when object with Rigidbody and colliders exits the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.SetBool("DoorOpen", false);
            
            // turn all lights red
            doorLight.material = doorOffMat; 
            stripe1.material = doorOffMat; 
            stripe2.material = doorOffMat; 
            stripe3.material = doorOffMat; 
            stripe4.material = doorOffMat; 
            playerInside = false;
            currentDoorWaitTime = 0f; // Reset wait time
        }
    }

    // trigger when something is spawned within the trigger
    // called every frame that other object is still inside the trigger
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void Update()
    {
        if (currentDoorWaitTime >= doorWaitTime)
        {
            // Open the door
            doorAnimator.SetBool("DoorOpen", true);
            return; //stop counting up number
        }
        if (playerInside)
        {
            // count up the wait time
            currentDoorWaitTime += Time.deltaTime;
        }
    }

}
