using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;

    [SerializeField] private float minX = -100f;
    [SerializeField] private float maxX = 100f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;

        // Clamp X position
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);

        // Keep Z position fixed for 2D
        desiredPosition.z = transform.position.z;

        // Smooth follow
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}