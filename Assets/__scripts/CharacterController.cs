using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class CharacterController : MonoBehaviour
{
    public Animator animator;
    public float walkSpeed = 3f;
    public float crouchSpeed = 1.5f;
    public float rotationSpeed = 10f;
    public LayerMask groundMask;

    public BoxCollider headCollider;    // Sphere collider near the head
    public BoxCollider waistCollider;   // Sphere collider at waist height

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private bool isCrouching = false;
    private bool objectCrouching = false;
    private float speed;
    private Vector3 moveDirection;
    
    private bool isPunching = false;
    public bool IsPunching => isPunching;
    
    public float punchDuration = 5f; // Duration of the punch animation
    private float punchTimer = 0f;
    public float gravity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Ensure colliders are enabled initially
        headCollider.enabled = true;
        waistCollider.enabled = true;
    }

    void Update()
    {
        HandleInput();
        UpdateAnimator();
        CheckForCrouch();
        
        if (isPunching)
        {
            punchTimer += Time.deltaTime;
            if (punchTimer >= punchDuration)
            {
                isPunching = false;
                punchTimer = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
        rb.AddForce(Vector3.down * gravity * rb.mass);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isPunching)
        {
            isPunching = true;
            animator.SetTrigger("Punch");
        }

        if (!isPunching)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Get movement input in world space
            moveDirection = new Vector3(horizontal, 0, vertical).normalized;

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCrouching = !isCrouching;
                AdjustColliderForCrouching();
            }

            speed = isCrouching ? crouchSpeed : walkSpeed;
        }
        else
        {
            moveDirection = Vector3.zero; // Stop movement while punching
        }
    }

    void AdjustColliderForCrouching()
    {
        if (isCrouching)
        {
            capsuleCollider.height = 0.8f;
            capsuleCollider.center = new Vector3(0, 0.4f, 0);
        }
        else
        {
            capsuleCollider.height = 1.9f;
            capsuleCollider.center = new Vector3(0, 0.9f, 0);
        }
    }

    void MovePlayer()
    {
        if (moveDirection.magnitude >= 0.05f)
        {
            // Move in the direction of the camera's forward direction
            Vector3 move = Camera.main.transform.TransformDirection(moveDirection);
            move.y = 0;  // Flatten the movement so that it doesn't affect the y-axis

            rb.MovePosition(rb.position + move * (speed * Time.fixedDeltaTime));

            RotatePlayer(move);  // Rotate to face the movement direction
        }
    }

    void RotatePlayer(Vector3 direction)
    {
        // Rotate the player to face the movement direction
        if (direction.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void UpdateAnimator()
    {
        float currentSpeed = moveDirection.magnitude * speed;
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isPunching", isPunching);
    }

    // Check if head is blocked but waist is not, to allow crouching
    void CheckForCrouch()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMaskWithoutPlayer = groundMask & ~(1 << playerLayer);  // Exclude player layer from groundMask

        // Perform OverlapSphere to detect if the head and waist hit any objects (excluding player layer)
        Collider[] headHits = Physics.OverlapBox(headCollider.transform.position + headCollider.center, headCollider.size / 2, Quaternion.identity, layerMaskWithoutPlayer);
        Collider[] waistHits = Physics.OverlapBox(waistCollider.transform.position + waistCollider.center, waistCollider.size / 2, Quaternion.identity, layerMaskWithoutPlayer);

        // Check if head is blocked
        bool headBlocked = headHits.Length > 0;

        // Check if waist is clear
        bool waistClear = waistHits.Length == 0;

        // Decide crouching based on headBlocked and waistClear
        if (headBlocked && waistClear)
        {
            isCrouching = true;
            objectCrouching = true;
            AdjustColliderForCrouching();
        }
        else if (!headBlocked && objectCrouching)
        {
            isCrouching = false;
            objectCrouching = false;
            AdjustColliderForCrouching();
        }
    }
}
