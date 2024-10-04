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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Make sure the sphere colliders are disabled initially
        headCollider.enabled = true;
        waistCollider.enabled = true;
    }

    void Update()
    {
        HandleInput();
        UpdateAnimator();
        RotatePlayer();
        CheckForCrouch();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            AdjustColliderForCrouching();
        }

        speed = isCrouching ? crouchSpeed : walkSpeed;
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
        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 move = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
            rb.MovePosition(rb.position + move * (speed * Time.fixedDeltaTime));
        }
    }

    void RotatePlayer()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void UpdateAnimator()
    {
        float currentSpeed = moveDirection.magnitude * speed;
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("isCrouching", isCrouching);
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
        if (headBlocked)
        {
            Debug.Log("Head is blocked by:");
            foreach (Collider hit in headHits)
            {
                Debug.Log(hit.gameObject.name);  // Log the name of each object that blocked the head
            }
        }

        // Check if waist is clear
        bool waistClear = waistHits.Length == 0;
        if (!waistClear)
        {
            Debug.Log("Waist is blocked by:");
            foreach (Collider hit in waistHits)
            {
                Debug.Log(hit.gameObject.name);  // Log the name of each object that blocked the waist
            }
        }

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
