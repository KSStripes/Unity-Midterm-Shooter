using UnityEngine;
// ...existing code...
[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{
    [Header("Look Controls")]
    private Vector2 lookDirection;
    [SerializeField] private float mouseSens; // adjust to sensitivity of the mouse

    [Header("Movement")]
    private Vector3 moveDirection;
    [SerializeField] private float moveSpeed;

    [Header("Jump Controls")]
    [SerializeField] private LayerMask groundLayer;
    private float gravityForce;
    private bool isGrounded;
    [SerializeField] private float jumpForce;

    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Rigidbody projectilePrefab;
    [SerializeField] private float shootingForce; 

    private CharacterController controller;
    private Camera head;

    private void Awake()
    {
        // look mouse to screen center
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        controller = GetComponent<CharacterController>();
        head = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Camera look direction
        lookDirection.x += Input.GetAxis("Mouse X") * Time.deltaTime * mouseSens;
        lookDirection.y -= Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSens;

        float angleOnY = lookDirection.y;
        // clamp player head up/down movement
        lookDirection.y = Mathf.Clamp(lookDirection.y, -80f, 80f);
        head.transform.localRotation = Quaternion.Euler(-lookDirection.y, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, lookDirection.x, 0f); //rotate player

        // Player movement 
        moveDirection.x = Input.GetAxis("Horizontal"); // left/right
        moveDirection.z = Input.GetAxis("Vertical"); // forward/backward

        Vector3 forwardMovement = moveDirection.z * transform.forward;
        Vector3 sidewaysMovement = moveDirection.x * transform.right;

        Vector3 moveVector = (forwardMovement + sidewaysMovement);

        // check if player is grounded
        isGrounded = Physics.CheckSphere(transform.position, 0.05f, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            gravityForce = jumpForce;
            isGrounded = false; // set grounded to false when jumping
        }

        controller.Move(moveVector * moveSpeed * Time.deltaTime);

        // jump movement
        if (!isGrounded)
        {
            gravityForce += -9f * Time.deltaTime;
            controller.Move(Vector3.up * gravityForce * Time.deltaTime); // apply gravity
        }
        else
        {
            gravityForce = 0f; //if grounded, gravity = 0
        }

        // shooting
        if (Input.GetMouseButtonDown(0))
        {
            Rigidbody clonedProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            clonedProjectile.AddForce(firePoint.forward * shootingForce, ForceMode.Impulse);
            Destroy(clonedProjectile.gameObject, 5f); // destroy projectile after 5 seconds
        }
    }

}
