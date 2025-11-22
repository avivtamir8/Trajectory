using UnityEngine;

/// <summary>
/// Controls a third-person camera, following the target object (Interceptor) with smooth movement and rotation.
/// It uses LateUpdate to ensure the target has moved for the current frame before the camera follows.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target & Offset")]
    [Tooltip("The Interceptor Sphere object to follow.")]
    public Transform target; 
    [Tooltip("The position offset (distance behind and above the target). The interceptor's forward is +X.")]
    public Vector3 offset = new Vector3(-100f, 50f, 0f); // Adjust these values in the Inspector if needed

    [Header("Damping")]
    [Tooltip("How smoothly the camera position catches up (lower is smoother).")]
    [Range(0.01f, 1f)]
    public float followSpeed = 0.125f; 
    [Tooltip("How smoothly the camera rotation catches up (lower is smoother).")]
    [Range(0.01f, 1f)]
    public float rotationSpeed = 0.5f;

    // Use LateUpdate to ensure the camera moves AFTER the target has moved in Update()
    void LateUpdate()
    {
        if (target == null)
        {
            // Fail gracefully if the target is not set
            return;
        }

        // 1. Calculate the desired camera position
        // Rotate the offset vector by the target's current rotation to keep it behind/above the missile
        Vector3 rotatedOffset = target.rotation * offset;
        Vector3 desiredPosition = target.position + rotatedOffset;

        // 2. Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed);

        // 3. Smoothly rotate the camera to look at the target
        // LookRotation defines a rotation that looks from the camera position toward the target position.
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed);
    }
}