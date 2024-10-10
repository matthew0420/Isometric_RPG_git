using UnityEngine;
using UnityEngine.AI;

public class Citizen : NPC
{
    public float walkRadius = 10f;

    protected override void Start()
    {
        base.Start();
        InvokeRepeating(nameof(ChooseRandomDestination), 0f, 5f);
    }

    // Citizen chooses a random place to walk
    private void ChooseRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
        {
            MoveTo(hit.position);
        }
    }

    // Citizens run away from the player
    public override void ReactToPlayer()
    {
        isRunning = true;
        animator.SetBool("isRunning", true);
        agent.speed = 7.0f; // Running speed
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized * 10f;
        MoveTo(transform.position + directionAwayFromPlayer);
    }
}