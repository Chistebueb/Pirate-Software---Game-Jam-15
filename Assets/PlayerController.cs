using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float length = 1.0f;

    void Update()
    {
        Vector3 newPosition = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            newPosition.y += length;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
        {
            newPosition.y -= length;
        }

        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            newPosition.x -= length;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition.x += length;
        }

        playerTransform.localPosition = newPosition;
    }
}
