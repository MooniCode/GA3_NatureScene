using UnityEngine;

public class WindmillRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f; // Degrees per second

    void Update()
    {
        // Rotate around local Z axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
