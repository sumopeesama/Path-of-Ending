using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    public Transform player; // Assign your player in the Inspector
    public Vector3 offset = new Vector3(0, 10, -5); // Adjust height and distance
    public float smoothSpeed = 5f; // Adjust for smoothness

    private void LateUpdate()
    {
        if (player == null) return;

        // Target position
        Vector3 targetPosition = player.position + offset;

        // Smoothly move the camera
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Keep the camera looking down
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}