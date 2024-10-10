using UnityEngine;
using UnityEngine.AI;

public class Citizen : NPC
{
    public float walkRadius = 10f;
    
    // Define bitmask for walkable areas excluding water
    private int normalAreaMask;  // Default area for wandering (e.g., exclude water)
    private int fleeingAreaMask; // Fleeing mask (allow water access)

    protected override void Start()
    {
        base.Start();
        
        // Set area masks
        normalAreaMask = NavMesh.AllAreas & ~(1 << NavMesh.GetAreaFromName("Water")); // Avoid water when wandering
        fleeingAreaMask = NavMesh.AllAreas; // Allow all areas (including water) when fleeing

        // Start wandering cycle
        InvokeRepeating(nameof(ChooseRandomDestination), 0f, 5f);
    }

    // Citizen chooses a random place to walk, avoiding water
    private void ChooseRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        // Use the normalAreaMask to avoid water when wandering
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, normalAreaMask))
        {
            MoveTo(hit.position);
        }
    }

    // Citizens run away from the player, allowing access to water
    public override void ReactToPlayer()
    {
        isRunning = true;
        animator.SetBool("isRunning", true);
        agent.speed = 7.0f; // Running speed

        // Change area mask to allow water when fleeing
        agent.areaMask = fleeingAreaMask;

        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized * 10f;
        MoveTo(transform.position + directionAwayFromPlayer);
    }

    // Reset to normal area mask after fleeing
    private void ResetAreaMask()
    {
        agent.areaMask = normalAreaMask;
    }

    protected override void Update()
    {
        base.Update();
    }
}