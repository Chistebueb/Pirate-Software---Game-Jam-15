using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    [SerializeField] private float range = 1.3f;
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private Transform deadZone;
    [SerializeField] private Transform legPoint;
    [SerializeField] private bool debugMode = false;
    [SerializeField] private float additionalDistance = 0.2f;
    [SerializeField] private float minLegWaitTime = 0.4f;
    [SerializeField] private float minDealyBetweenLegs = 0.2f;

    private Vector2 deadZoneSize;
    private float timer= 0f;
    private static float staticTimer = 0f;
    private bool hasRecentlyMovedLeg = false;


    void Start()
    {
        Renderer deadZoneRenderer = deadZone.GetComponent<Renderer>();

        BoxCollider2D deadZoneCollider = deadZone.GetComponent<BoxCollider2D>();
        if (deadZoneCollider != null)
        {
            deadZoneSize = deadZoneCollider.size;
        }
        else
        {
            Debug.LogError("DeadZone Transform does not have a BoxCollider2D component.");
        }

        SetLegPosition();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (hasRecentlyMovedLeg)
        {
            staticTimer += Time.deltaTime;
            if (staticTimer >= minDealyBetweenLegs)
            {
                hasRecentlyMovedLeg = false;
                staticTimer= 0f;
            }
        }

        BoxCollider2D deadZoneCollider = deadZone.GetComponent<BoxCollider2D>();
        deadZoneSize = deadZoneCollider.size;

        if ((IsInDeadZone(legPoint.position) || (IsOutsideRadius(legPoint.position) && staticTimer == 0f)) && timer >= minLegWaitTime)
        {
            timer= 0;

            if (IsOutsideRadius(legPoint.position))
            {
                hasRecentlyMovedLeg = true;
                staticTimer += Time.deltaTime;
            }

            SetLegPosition();
        }

        if (debugMode)
        {
            DrawAll();
        }
    }

    public void SetLegPosition()
    {
        Vector3 vectorDest = destinationPoint.position;
        Vector3 vectorJoint = transform.position;
        Vector3 interceptionPoint = CalculateInerseption(vectorDest, vectorJoint);
        interceptionPoint = MoveOutsideDeadzone(interceptionPoint);

        legPoint.position = interceptionPoint;
    }

    private Vector3 CalculateInerseption(Vector3 dest, Vector3 joint)
    {
        float diffY = joint.y - dest.y;
        float diffX = joint.x - dest.x;

        float hypo = (float) Math.Sqrt(diffX* diffX + diffY * diffY);
        float relation = range / hypo;

        return new Vector3(joint.x - diffX * relation, joint.y - diffY * relation, 0);
    }

    private Vector3 MoveOutsideDeadzone(Vector3 pos)
    {
        if (IsInDeadZone(pos))
        {
            // Transform the position to the local space of the deadZone
            Vector3 localPos = deadZone.InverseTransformPoint(pos);

            // Find the closest point on the edge of the dead zone
            float closestX = Mathf.Clamp(localPos.x, -deadZoneSize.x / 2, deadZoneSize.x / 2);
            float closestY = Mathf.Clamp(localPos.y, -deadZoneSize.y / 2, deadZoneSize.y / 2);

            // Determine the distance to each edge
            float distanceToLeft = Mathf.Abs(localPos.x + deadZoneSize.x / 2);
            float distanceToRight = Mathf.Abs(localPos.x - deadZoneSize.x / 2);
            float distanceToBottom = Mathf.Abs(localPos.y + deadZoneSize.y / 2);
            float distanceToTop = Mathf.Abs(localPos.y - deadZoneSize.y / 2);

            // Find the closest edge and adjust the position accordingly
            if (distanceToLeft < distanceToRight && distanceToLeft < distanceToBottom && distanceToLeft < distanceToTop)
            {
                closestX = -deadZoneSize.x / 2;
            }
            else if (distanceToRight < distanceToLeft && distanceToRight < distanceToBottom && distanceToRight < distanceToTop)
            {
                closestX = deadZoneSize.x / 2;
            }
            else if (distanceToBottom < distanceToLeft && distanceToBottom < distanceToRight && distanceToBottom < distanceToTop)
            {
                closestY = -deadZoneSize.y / 2;
            }
            else
            {
                closestY = deadZoneSize.y / 2;
            }

            Vector3 closestPointLocal = new Vector3(closestX, closestY, 0);
            Vector3 closestPointWorld = deadZone.TransformPoint(closestPointLocal);

            Vector3 directionAwayFromDestination = (closestPointWorld - destinationPoint.position).normalized;
            closestPointWorld += directionAwayFromDestination * additionalDistance;

            return closestPointWorld;
        }
        return pos;
    }

    private bool IsInDeadZone(Vector3 pos)
    {
        Vector3 localPos = deadZone.InverseTransformPoint(pos);

        return (localPos.x >= -deadZoneSize.x / 2 && localPos.x <= deadZoneSize.x / 2 &&
                localPos.y >= -deadZoneSize.y / 2 && localPos.y <= deadZoneSize.y / 2);
    }

    private bool IsOutsideRadius(Vector3 pos)
    {
        Vector3 vectorJoint = transform.position;
        float diffY = vectorJoint.y - pos.y;
        float diffX = vectorJoint.x - pos.x;

        float distance = Mathf.Sqrt(diffX * diffX + diffY * diffY);

        return distance > range;
    }

    public void DrawAll()
    {
        Vector3 vectorDest = destinationPoint.position;
        Vector3 vectorJoint = transform.position;

        DrawCircle(vectorJoint, 0.2f, 32, Color.green);
        DrawCircle(vectorJoint, range, 32, Color.red);

        Vector3 interceptionPoint = CalculateInerseption(vectorDest, vectorJoint);
        interceptionPoint = MoveOutsideDeadzone(interceptionPoint);

        DrawCircle(interceptionPoint, 0.2f, 32, Color.green);
        DrawCircle(destinationPoint.position, 0.2f, 32, Color.red);

        DrawCircle(legPoint.position, 0.2f, 32, Color.blue);
    }

    public static void DrawCircle(Vector3 position, float radius, int segments, Color color)
    {
        // If either radius or number of segments are less or equal to 0, skip drawing
        if (radius <= 0.0f || segments <= 0)
        {
            return;
        }

        // Single segment of the circle covers (360 / number of segments) degrees
        float angleStep = (360.0f / segments);

        // Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
        // which are required by Unity's Mathf class trigonometry methods

        angleStep *= Mathf.Deg2Rad;

        // lineStart and lineEnd variables are declared outside of the following for loop
        Vector3 lineStart = Vector3.zero;
        Vector3 lineEnd = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            // Line start is defined as starting angle of the current segment (i)
            lineStart.x = Mathf.Cos(angleStep * i);
            lineStart.y = Mathf.Sin(angleStep * i);

            // Line end is defined by the angle of the next segment (i+1)
            lineEnd.x = Mathf.Cos(angleStep * (i + 1));
            lineEnd.y = Mathf.Sin(angleStep * (i + 1));

            // Results are multiplied so they match the desired radius
            lineStart *= radius;
            lineEnd *= radius;

            // Results are offset by the desired position/origin 
            lineStart += position;
            lineEnd += position;

            // Points are connected using DrawLine method and using the passed color
            Debug.DrawLine(lineStart, lineEnd, color);
        }
    }
}
