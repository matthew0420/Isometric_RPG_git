using UnityEngine;
using System.Collections.Generic;

public class FishBoid : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float neighborDistance = 2f;
    public float bounceBackForce = 10f; // Strength of the bounce back force when hitting the boundary

    private Vector3 velocity;
    private List<FishBoid> allFish;
    private Vector3 waterSurface; // Set the y position of the water surface
    private float swimDepth = 1.0f; // How deep the fish can swim

    public BoxCollider waterCollider; // Reference to the BoxCollider

    void Start()
    {
        velocity = transform.forward * speed;
        allFish = new List<FishBoid>(FindObjectsOfType<FishBoid>());
        waterSurface = new Vector3(transform.position.x, transform.position.y + swimDepth, transform.position.z);
    }

    void Update()
    {
        Vector3 acceleration = Vector3.zero;
        Vector3 position = transform.position;

        if (position.y < waterSurface.y)
        {
            // If fish is below water surface, apply the Boid behaviors
            Vector3 cohesion = Cohesion();
            Vector3 separation = Separation();
            Vector3 alignment = Alignment();

            acceleration += cohesion + separation + alignment;
        }
        else
        {
            // If fish is above the water surface, force it to swim back down
            acceleration += (waterSurface - position).normalized * speed;
        }

        // Update velocity and position
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, speed); // Limit speed

        // Move the fish
        transform.position += velocity * Time.deltaTime;

        // Check if the fish is outside the water collider
        KeepFishInBounds();

        // Rotate towards the direction of movement
        if (velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void KeepFishInBounds()
    {
        Vector3 position = transform.position;
        
        // Check if the fish is outside the BoxCollider
        if (!waterCollider.bounds.Contains(position))
        {
            // Get the closest point on the BoxCollider bounds
            Vector3 closestPoint = waterCollider.ClosestPoint(position);
            // Calculate the direction to move the fish back inside the collider
            Vector3 directionToInside = (closestPoint - position).normalized;

            // Apply a bounce back force to the fish
            velocity += directionToInside * bounceBackForce * Time.deltaTime;

            // Move the fish to the closest point inside the collider to prevent stacking
            transform.position = closestPoint;
        }
    }

    Vector3 Cohesion()
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (FishBoid fish in allFish)
        {
            if (fish != this && Vector3.Distance(transform.position, fish.transform.position) < neighborDistance)
            {
                center += fish.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            center /= count;
            return (center - transform.position).normalized * speed * 0.1f; // Move towards the center
        }

        return Vector3.zero;
    }

    Vector3 Separation()
    {
        Vector3 moveAway = Vector3.zero;

        foreach (FishBoid fish in allFish)
        {
            if (fish != this && Vector3.Distance(transform.position, fish.transform.position) < neighborDistance)
            {
                // Calculate the vector to move away from the other fish
                moveAway += transform.position - fish.transform.position;
            }
        }

        // Add weight to the separation to prevent stacking
        return moveAway * 1.5f; // Increase this value to make separation stronger
    }

    Vector3 Alignment()
    {
        Vector3 averageHeading = Vector3.zero;
        int count = 0;

        foreach (FishBoid fish in allFish)
        {
            if (fish != this && Vector3.Distance(transform.position, fish.transform.position) < neighborDistance)
            {
                averageHeading += fish.velocity;
                count++;
            }
        }

        if (count > 0)
        {
            averageHeading /= count;
            return (averageHeading.normalized * speed - velocity) * 0.1f; // Steer towards the average heading
        }

        return Vector3.zero;
    }
}
