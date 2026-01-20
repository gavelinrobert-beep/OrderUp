using UnityEngine;

namespace OrderUp.Player
{
    /// <summary>
    /// CameraFollow provides smooth camera tracking for the local player
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -8f);
        [SerializeField] private float smoothSpeed = 0.125f;
        [SerializeField] private float lookAtHeight = 1f;

        private Transform target;

        /// <summary>
        /// Sets the target for the camera to follow
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            Debug.Log($"CameraFollow: Target set to {newTarget.name}");
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Calculate desired position
            Vector3 desiredPosition = target.position + offset;
            
            // Smoothly interpolate to desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // Look at target with offset height
            Vector3 lookAtPosition = target.position + Vector3.up * lookAtHeight;
            transform.LookAt(lookAtPosition);
        }
    }
}
