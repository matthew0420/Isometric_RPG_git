using UnityEngine;

public class FishWander : MonoBehaviour
{
    public float speed = 2f; // Speed of the fish
    public float changeDirectionInterval = 2f; // Time interval to change direction
    public BoxCollider pondCollider; // Reference to the BoxCollider that defines the pond area

    private Vector3 targetPosition;

    private void Start()
    {
        // Set the initial target position
        SetRandomTargetPosition();

        // Start changing direction at intervals
        InvokeRepeating(nameof(SetRandomTargetPosition), changeDirectionInterval, changeDirectionInterval);
    }

    private void Update()
    {
        // Move towards the target position
        MoveTowardsTarget();
    }

    private void SetRandomTargetPosition()
    {
        if (pondCollider != null)
        {
            // Get the bounds of the collider
            Vector3 center = pondCollider.center + pondCollider.transform.position;
            Vector3 size = pondCollider.size;

            // Generate a random point within the bounds of the BoxCollider
            float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
            float randomZ = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
            targetPosition = new Vector3(randomX, transform.position.y, randomZ);
        }
        else
        {
            Debug.LogWarning("Pond Collider not assigned!");
        }
    }

    private void MoveTowardsTarget()
    {
        // Move the fish towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Optional: Rotate the fish to face the movement direction
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed * 2);
        }

        // Check if the fish reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Set a new target position if close to the current target
            SetRandomTargetPosition();
        }
    }
}
