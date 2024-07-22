using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawLine : MonoBehaviour
{
    // Serialized fields to assign the transforms in the Unity Editor
    public Transform startPoint;
    public Transform endPoint;

    private LineRenderer lineRenderer;

    void Start()
    {
        // Initialize the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        // Update the positions to draw the line between every frame
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }

    // Configure basic properties of the LineRenderer
    private void SetupLineRenderer()
    {
        // Set the number of positions to 2 (start and end)
        lineRenderer.positionCount = 2;

        // Optionally set width and color
        //lineRenderer.startWidth = 0.1f;  // Example width
        //lineRenderer.endWidth = 0.08f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"))
        {
            color = Color.white
        };
    }
}
