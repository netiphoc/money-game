using UnityEngine;

public class CameraFace : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("If true, the object will only rotate on the Y axis (stay upright).")]
    [SerializeField] private bool lockYAxis = false;

    [Tooltip("If true, the object flips 180 degrees. Useful for UI Text or Sprites that look backwards.")]
    [SerializeField] private bool invertFace = false;

    private Camera _mainCamera;

    private void Start()
    {
        // Cache the camera for performance
        _mainCamera = Camera.main;
        
        if (_mainCamera == null)
        {
            Debug.LogError("No Main Camera found! Tag your camera as 'MainCamera'.");
        }
    }

    // specific: LateUpdate ensures we rotate AFTER the camera has moved 
    // to prevent jittering during movement.
    private void LateUpdate()
    {
        if (_mainCamera == null) return;

        // 1. Determine the target position
        Vector3 targetPosition = _mainCamera.transform.position;

        // 2. Adjust for Y-Axis locking
        if (lockYAxis)
        {
            // Keep the object's own Y height, ignoring the camera's height
            targetPosition.y = transform.position.y;
        }

        // 3. Perform the rotation
        // By default, LookAt points the "Forward" (Z) vector at the target
        if (invertFace)
        {
            // Calculate the direction FROM camera TO object (opposite of looking at camera)
            Vector3 directionAwayFromCamera = transform.position - targetPosition;
            
            // If locking Y, flatten the direction vector
            if (lockYAxis) directionAwayFromCamera.y = 0;

            if (directionAwayFromCamera != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionAwayFromCamera);
            }
        }
        else
        {
            transform.LookAt(targetPosition);
        }
    }
}