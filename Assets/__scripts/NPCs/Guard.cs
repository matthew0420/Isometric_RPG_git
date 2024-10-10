using UnityEngine;
using UnityEngine.AI;

public class Guard : NPC
{
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    public GameObject waterRipple;
    protected override void Start()
    {
        base.Start();
        MoveTo(patrolPoints[currentPatrolIndex].position);
    }

    // patrol route
    private void Patrol()
    {
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            MoveTo(patrolPoints[currentPatrolIndex].position);
        }
    }

    // Guards run towards the player
    public override void ReactToPlayer()
    {
        isRunning = true;
        animator.SetBool("isRunning", true);
        agent.speed = 7.0f; // Running speed
        MoveTo(player.position);
    }

    protected override void Update()
    {
        base.Update();
        Patrol();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            waterRipple.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
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
        if (other.gameObject.CompareTag("Water"))
        {
            waterRipple.SetActive(false);
        }
    }
}