using UnityEngine;

public class UpAndDownAnimation : MonoBehaviour
{
    public float movementDistance = 0.5f; // The distance the object will move up and down
    public float movementSpeed = 1.0f; // The speed of the movement
    public float rotationSpeed = 45.0f; // The speed of the rotation

    private Vector3 initialPosition;
    private bool movingUp = true;

    void Start()
    {
        // Store the initial position of the object
        initialPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new position based on the movement direction and speed
        Vector3 newPosition = transform.position;
        float movementStep = movementSpeed * Time.deltaTime;

        if (movingUp)
        {
            newPosition.y += movementStep;
        }
        else
        {
            newPosition.y -= movementStep;
        }

        // If the object has reached the maximum or minimum height, change the movement direction
        if (Mathf.Abs(newPosition.y - initialPosition.y) >= movementDistance)
        {
            movingUp = !movingUp;
        }

        // Update the object's position
        transform.position = newPosition;

        // Rotate the object
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
