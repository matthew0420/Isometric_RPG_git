using UnityEngine;
using Random = UnityEngine.Random;

public class FishWander : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float turnSpeed = 2f;
    public float directionChangeInterval = 3f;
    public float fleeDistance = 5f;
    public BoxCollider pondBounds;

    private float initialY;
    private float timer;
    private Vector3 targetDirection;
    
    void Start()
    {
        //for pond fish we will keep their height the same, perhaps different kinds of fish would have different wander behavior
        initialY = transform.position.y;
        ChangeDirection();
    }

    void Update()
    {
        CheckForFlee();
        transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        timer += Time.deltaTime;
        if (timer >= directionChangeInterval)
        {
            ChangeDirection();
            timer = 0f;
        }

        KeepWithinBounds();
    }

    void ChangeDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        targetDirection = new Vector3(Mathf.Sin(randomAngle), 0, Mathf.Cos(randomAngle));
    }

    void CheckForFlee()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, fleeDistance);
        foreach (var hitCollider in hitColliders)
        {
            Transform rippleParticle = hitCollider.transform.Find("WaterRippleParticle");
            if (rippleParticle != null && rippleParticle.gameObject.activeInHierarchy)
            {
                Vector3 fleeDirection = (transform.position - hitCollider.transform.position).normalized;
                Vector3 potentialNewPosition = transform.position + fleeDirection * (moveSpeed * Time.deltaTime);
                Bounds bounds = pondBounds.bounds;
                if (!bounds.Contains(potentialNewPosition))
                {
                    Vector3 closestPoint = bounds.ClosestPoint(transform.position);
                    Vector3 adjustedDirection = (closestPoint - transform.position).normalized;

                    targetDirection = adjustedDirection;
                }
                else
                {
                    targetDirection = fleeDirection;
                }

                break; // Flee from the closest dangerous object found
            }
        }
    }

    void KeepWithinBounds()
    {
        Bounds bounds = pondBounds.bounds;

        if (!bounds.Contains(transform.position))
        {
            Vector3 centerOffset = (bounds.center - transform.position).normalized;
            targetDirection = new Vector3(centerOffset.x, 0, centerOffset.z);
        }
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
    }
}