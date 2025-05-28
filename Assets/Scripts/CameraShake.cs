using UnityEngine;

public class SmoothCameraMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How high/low the camera will move from its starting position")]
    [SerializeField] private float amplitude = 0.5f;

    [Tooltip("How fast the camera completes a full movement cycle")]
    [SerializeField] private float frequency = 1.0f;

    [Tooltip("Offset the sine wave to start at a different position")]
    [SerializeField] private float phaseOffset = 0f;

    // Store the camera's starting position
    private Vector3 startPosition;

    void Start()
    {
        // Capture the initial position of the camera
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the sine wave value based on time
        float sineWave = Mathf.Sin((Time.time * frequency) + phaseOffset) * amplitude;

        // Create a new position with the original x and z, but modified y
        Vector3 newPosition = new Vector3(
            startPosition.x,
            startPosition.y + sineWave,
            startPosition.z
        );

        // Apply the new position to the camera
        transform.position = newPosition;
    }
}