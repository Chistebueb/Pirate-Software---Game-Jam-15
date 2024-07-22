using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target; // Target to follow
    public float speed = 5f; // Maximum speed at which the object should follow the target
    public float stopDistance = 1f; // Distance at which the object stops moving towards the target

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing from this game object");
        }
    }

    void FixedUpdate()
    {
        if (target != null && rb != null)
        {
            Vector2 direction = (target.position - transform.position);
            float distance = direction.magnitude;

            // Check if the object is within the stopping distance
            if (distance < stopDistance)
            {
                rb.velocity = Vector2.zero; // Stop the object's movement
            }
            else
            {
                // Normalize the direction vector and calculate scaled speed
                direction.Normalize();
                float scaledSpeed = speed * (distance / stopDistance); // Slow down as it gets closer
                scaledSpeed = Mathf.Min(scaledSpeed, speed); // Ensure the speed does not exceed the maximum speed

                rb.velocity = direction * scaledSpeed;
            }
        }
    }
}
