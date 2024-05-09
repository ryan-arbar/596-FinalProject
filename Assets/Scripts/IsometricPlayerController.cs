using UnityEngine;

public class IsometricPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 7f;
    public bool canRun = false;
    public bool canJump = false;
    public bool canRotateCamera = true;

    [Header("Ground Check Parameters")]
    [SerializeField] private float checkRadius = 0.4f;
    [SerializeField] private float groundCheckDistance = 0.2f; // Distance for ground check

    [Header("Audio")]
    public AudioClip jumpSound;
    private AudioSource audioSource;

    [Header("Debug")]
    [SerializeField] private bool isGrounded;

    private Rigidbody rb;
    private Vector3 forward, right;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
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
        // Update movement based on the camera's orientation
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
            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound); // Play the jump sound effect
            }
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
