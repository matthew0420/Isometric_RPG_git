using UnityEngine;

//Guard state extension:
//guards could knock out threats, then begin patrolling again
//guards could stop attacking after assailant has fled a certain distance
public class Guard : NPC
{
    public Transform[] patrolPoints;
    public GameObject waterRipple;
    // Player detection and attack settings
    public LayerMask detectionLayer;
    public float detectionRadius = 10.0f; // Detection radius for player
    public float attackRange = 2.0f;
    public float attackCooldown = 2.0f;
    
    private float nextAttackTime = 0f;
    private int currentPatrolIndex = 0;

    // States
    private enum GuardState { Patrolling, Attacking }
    private GuardState currentState = GuardState.Patrolling;

    protected override void Start()
    {
        base.Start();
        MoveTo(patrolPoints[currentPatrolIndex].position);
    }
    
    private void Patrol()
    {
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            MoveTo(patrolPoints[currentPatrolIndex].position);
        }
    }

    public override void ReactToPlayer()
    {
        // ReactToPlayer can be used if the guard notices the player through other means (like noise or sight)
        // For now, this switches the guard's state to Attacking
        currentState = GuardState.Attacking;
        isRunning = true;
        agent.speed = 7.0f;
        MoveTo(player.position);
    }
    
    public void FollowAndAttackPlayer()
    {
        MoveTo(player.position);

        // Check if within attack range and attack cooldown, animation to be added
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void AttackPlayer()
    {
        //animator.SetTrigger("punch");
        Debug.Log("Attacking player!");
        // Add logic to damage the player and use attack animation when in range
    }

    protected override void Update()
    {
        base.Update();
        
        switch (currentState)
        {
            case GuardState.Patrolling:
                Patrol();
                break;
            case GuardState.Attacking:
                FollowAndAttackPlayer();
                break;
        }
    }
    
    public void NotifyToAttackPlayer(Transform playerTransform)
    {
        player = playerTransform;  
        currentState = GuardState.Attacking;  
    }

    //if player can escape, code goes here to handle reset
    public void NotifyToStopAttacking()
    {
        currentState = GuardState.Patrolling; 
    }

    // Handle water ripples 
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            if (agent.velocity.magnitude == 0)
            {
                waterRipple.SetActive(false);
            }
            else
            {
                waterRipple.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterRipple.SetActive(false);
        }
    }
}
