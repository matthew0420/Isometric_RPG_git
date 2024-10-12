using UnityEngine;
using UnityEngine.AI;

//More interactions can be defined later, such as an Enum for Citizen state:
//fleeing state = running away
//hiding state = comes after fleeing, unique hide animation
//timer runs...
//if threat is gone or time has passed, begin wandering again in wandering state
public class Citizen : NPC
{
    public float walkRadius = 50f;
    public float fleeDistance = 200f;
    public GameObject waterRipple;
    
    // Define bitmask for walkable areas excluding water
    private int normalAreaMask; 
    private int fleeingAreaMask; 
    
    protected override void Start()
    {
        base.Start();
        
        normalAreaMask = NavMesh.AllAreas & ~(1 << NavMesh.GetAreaFromName("Water"));
        fleeingAreaMask = NavMesh.AllAreas;
        
        InvokeRepeating(nameof(ChooseRandomDestination), 0f, 5f);
    }
    
    private void ChooseRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, normalAreaMask))
        {
            MoveTo(hit.position);
        }
    }
    
    private void ResetAreaMask()
    {
        agent.areaMask = normalAreaMask;
    }
    
    public void FleeFromPlayer(Transform playerTransform)
    {
        CancelInvoke();
        isRunning = true;
        agent.speed = 4.0f;
        
        agent.areaMask = fleeingAreaMask;
        Vector3 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;
        Vector3 fleePosition = transform.position + directionAwayFromPlayer * fleeDistance;
        
        if (IsPlayerLookingAtPosition(fleePosition, playerTransform))
        {
            fleePosition = FindHiddenPosition(playerTransform);
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePosition, out hit, fleeDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position); 
        }
    }
    
    private bool IsPlayerLookingAtPosition(Vector3 position, Transform playerTransform)
    {
        Vector3 toPosition = (position - playerTransform.position).normalized;
        float dotProduct = Vector3.Dot(playerTransform.forward, toPosition);

        // Consider positions in front of the player as within FoV (using a threshold)
        return dotProduct > 0.5f; 
    }
    
    private Vector3 FindHiddenPosition(Transform playerTransform)
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        
        if (Vector3.Dot(playerTransform.forward, randomDirection) > 0)
        {
            randomDirection = -randomDirection;
        }

        return transform.position + randomDirection * fleeDistance;
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
