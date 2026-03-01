using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    public Transform player; // Assign your player GameObject in the Inspector
    public float smoothSpeed = 5f; // Adjust for smoother or snappier follow
    public float mouseWeight = 0.5f; // Determines how much the camera moves towards the mouse (0 = none, 1 = only mouse)

    private Vector3 desiredPosition;
    private Vector3 velocity = Vector3.zero;

    void FixedUpdate() {
        if (player == null) return;

        // Get mouse position in world space
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // In 2D, the Z value is often 0 or a fixed offset, adjust as needed
        mousePosition.z = 0;

        // Calculate a point between the player and the mouse
        // The mouseWeight determines the blend. A weight of 0.5 is the exact midpoint.
        Vector3 targetPoint = Vector3.Lerp(player.position, mousePosition, mouseWeight);

        // Maintain the camera's original Z position (usually -10 for 2D)
        targetPoint.z = transform.position.z;

        // Smoothly move the camera to the target point
        transform.position = Vector3.SmoothDamp(transform.position, targetPoint, ref velocity, smoothSpeed * Time.deltaTime);
    }
}
