using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] private float speed = 1.3f;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private KeyCode liftKey = KeyCode.P; //move lift with P

    private bool isMoving = false;
    private Vector3 startPos;
    private Vector3 currentPosition;
    private Vector3 targetPos;

    private bool atStart = true;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(liftKey))
        {
            ToggleLift();
        }
    }


    public void ToggleLift()
    {
        if (isMoving) return;

        if (atStart)
        {
            targetPos = targetPosition.position;
            atStart = false;
            isMoving = true;
        }
        else
        {
            targetPos = startPos;
            atStart = true;
            isMoving = true;
        }
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;

        // lerp to the target position if is moving is true
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.fixedDeltaTime * speed);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            isMoving = false;
        }
    }
}
