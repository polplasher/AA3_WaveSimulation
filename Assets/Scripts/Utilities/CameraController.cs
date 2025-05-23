using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = Vector3.zero;

    [Header("Orbit Controls")]
    [SerializeField] private float distance = 10.0f;
    [SerializeField] private float minDistance = 2.0f;
    [SerializeField] private float maxDistance = 20.0f;
    [SerializeField] private float xSpeed = 250.0f;
    [SerializeField] private float ySpeed = 120.0f;
    [SerializeField] private float zoomSpeed = 5.0f;

    [Header("Orbit Limits")]
    [SerializeField] private float yMinLimit = -20f;
    [SerializeField] private float yMaxLimit = 80f;

    [Header("Damping")]
    [SerializeField] private float damping = 5.0f;
    [SerializeField] private bool smoothRotation = true;

    // Internal variables
    private float x = 0.0f;
    private float y = 0.0f;
    private Quaternion rotation;
    private Vector3 desiredPosition;
    private Vector3 position;

    private void Start()
    {
        // If no target is set, try to find a suitable target
        if (target == null)
        {
            // Look for buoys or other suitable objects
            var buoy = FindObjectOfType<Buoyancy.Buoy>();
            if (buoy != null)
            {
                target = buoy.transform;
            }
        }

        // Set initial camera angles based on current rotation
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rotation = Quaternion.Euler(y, x, 0);
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Get mouse input for rotation
        if (Input.GetMouseButton(0))  // Left mouse button for rotation
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            // Clamp vertical rotation
            y = ClampAngle(y, yMinLimit, yMaxLimit);
        }

        // Get mouse wheel input for zooming
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Calculate desired rotation
        Quaternion targetRotation = Quaternion.Euler(y, x, 0);

        // Apply damping to rotation if enabled
        if (smoothRotation)
        {
            rotation = Quaternion.Slerp(rotation, targetRotation, damping * Time.deltaTime);
        }
        else
        {
            rotation = targetRotation;
        }

        // Calculate camera position based on distance, rotation and target
        Vector3 targetPos = target.position + targetOffset;
        desiredPosition = targetPos - (rotation * Vector3.forward * distance);

        // Apply damping to position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, damping * Time.deltaTime);

        // Look at the target
        transform.rotation = rotation;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    // Helper method to manually set a new target
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Helper method to reset camera position
    public void ResetCamera()
    {
        x = 0;
        y = 0;
        distance = 10.0f;
    }
}
