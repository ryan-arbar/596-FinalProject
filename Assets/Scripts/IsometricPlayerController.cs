using UnityEngine;

public class IsometricPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 10f; // Run speed
    public float jumpForce = 7f; // Jump force
    public float kbForce; // knockback force
    public bool canRun = false; // Flag for running ability
    public bool canJump = false; // Flag for jumping ability
    public bool canRotateCamera = true;

    [Header("Ground Check Parameters")]
    [SerializeField] private float checkRadius = 0.4f; // Radius for ground check
    [SerializeField] private float groundCheckDistance = 0.2f; // Distance for ground check

    [Header("Debug")]
    [SerializeField] private bool isGrounded;

    private Rigidbody rb;
    private Vector3 forward, right;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        SetMovementVectors();
        HandleMovement();
        HandleJump();
        CheckGroundedStatus();
        
    }

    private void SetMovementVectors()
    {
        // Update movement vectors based on the camera's orientation
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

    void CheckGroundedStatus()
    {
        Vector3 spherePosition = transform.position - Vector3.down * 0.4f;
        bool hitDetected = Physics.SphereCast(spherePosition, checkRadius, Vector3.down, out _, groundCheckDistance);
        isGrounded = hitDetected;
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = (horizontalInput * right + verticalInput * forward).normalized;
        float currentSpeed = canRun && Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);

        // Face the direction of movement
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
        }
    }

    private void HandleJump()
    {
        if (canJump && Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        
        // Check if the colliding object is tagged as an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Has collided");
            // Calculate the knockback direction away from the enemy
            Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;

            // Apply the knockback force to the player rigidbody
            rb.AddForce(knockbackDirection * kbForce, ForceMode.Impulse);
        }
    }

    public void EnableRunning()
    {
        canRun = true;
    }

    public void EnableJumping()
    {
        canJump = true;
    }

    public void EnableCameraRotation()
    {
        canRotateCamera = true;
    }

}
