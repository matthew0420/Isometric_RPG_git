using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class CharacterController : MonoBehaviour
{
    public Animator animator;
    public float walkSpeed = 3f;
    public float crouchSpeed = 1.5f;
    public float rotationSpeed = 10f;
    public LayerMask groundMask;

    
    //since environment state machine handles hand interactions, and handles multiple stages of states,
    //I am doing foot IK here since it will act independently of what hand IK is doing
    //I would likely have an EnvironmentInteractionHand and EnvironmentInteractionFoot setup for state machines if I worked on this further
    public Transform leftFoot; 
    public Transform rightFoot; 
    public float footOffset = 0.1f;  
    public float footIKWeight = 1.0f; 
    public float raycastDistance = 1.0f;

    public BoxCollider headCollider;   
    public BoxCollider waistCollider;  

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private bool isCrouching = false;
    private bool objectCrouching = false;
    private float speed;
    private Vector3 moveDirection;
    
    private bool isPunching = false;
    public bool IsPunching => isPunching;
    
    public float punchDuration = 5f; 
    private float punchTimer = 0f;
    public float gravity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
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
            capsuleCollider.height = 1.3f;
            capsuleCollider.center = new Vector3(0, 0.65f, 0);
        }
        else
        {
            capsuleCollider.height = 1.85f;
            capsuleCollider.center = new Vector3(0, 0.92f, 0);
        }
    }

    void MovePlayer()
    {
        if (moveDirection.magnitude >= 0.05f)
        {
            // Move in the direction of the camera's forward direction for isometric feel
            Vector3 move = Camera.main.transform.TransformDirection(moveDirection);
            move.y = 0;  

            rb.MovePosition(rb.position + move * (speed * Time.fixedDeltaTime));

            RotatePlayer(move); 
        }
    }

    void RotatePlayer(Vector3 direction)
    {
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
    
    void OnAnimatorIK(int layerIndex)
    {
        AdjustFootPosition(leftFoot, AvatarIKGoal.LeftFoot);
        AdjustFootPosition(rightFoot, AvatarIKGoal.RightFoot);
    }
    
    void AdjustFootPosition(Transform footTransform, AvatarIKGoal footGoal)
    {
        RaycastHit hit;
        Vector3 footPosition = footTransform.position + Vector3.up * raycastDistance;
        
        if (Physics.Raycast(footPosition, Vector3.down, out hit, raycastDistance + footOffset, groundMask))
        {
            Vector3 targetFootPosition = hit.point;
            targetFootPosition.y += footOffset; 
            
            animator.SetIKPositionWeight(footGoal, footIKWeight);
            animator.SetIKPosition(footGoal, targetFootPosition);

            // Calculate the foot's rotation to match the slope's normal
            Vector3 footForward = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;
            Quaternion targetFootRotation = Quaternion.LookRotation(footForward, hit.normal);
            
            animator.SetIKRotationWeight(footGoal, footIKWeight);
            animator.SetIKRotation(footGoal, targetFootRotation);
        }
        else
        {
            animator.SetIKPositionWeight(footGoal, 0);
            animator.SetIKRotationWeight(footGoal, 0);
        }
    }
    
    // Check if head is blocked but waist is not, to allow crouching
    void CheckForCrouch()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMaskWithoutPlayer = groundMask & ~(1 << playerLayer); 
        
        Collider[] headHits = Physics.OverlapBox(headCollider.transform.position + headCollider.center, headCollider.size / 2, Quaternion.identity, layerMaskWithoutPlayer);
        Collider[] waistHits = Physics.OverlapBox(waistCollider.transform.position + waistCollider.center, waistCollider.size / 2, Quaternion.identity, layerMaskWithoutPlayer);
        
        bool headBlocked = headHits.Length > 0;
        
        bool waistClear = waistHits.Length == 0;
        
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
