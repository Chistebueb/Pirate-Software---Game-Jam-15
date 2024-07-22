using System.Collections;
using UnityEngine;

public class SmoothTurnTowardsTarget : MonoBehaviour
{
    public Transform target; 
    public float turnSpeed = 1.0f;
    public float speedThreshold = 5.0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rb.velocity.magnitude > speedThreshold)
        {
            StartCoroutine(SmoothTurn());
        }
    }

    IEnumerator SmoothTurn()
    {
        Vector3 savedPosition = target.localPosition;
        while (true)
        {
            Vector3 currentDirection = target.position - transform.position;

            float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);



            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f || !savedPosition.Equals(target.localPosition) || (target.localPosition.y == 0 && target.localPosition.x == 0))
            {
                Debug.Log("a");
                yield break;
            }

            yield return null;
        }
    }
}