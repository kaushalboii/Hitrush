using UnityEngine;

namespace Hitrush.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        
        [Header("Follow Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 5, -8);
        [SerializeField] private float followSpeed = 10f;
        [SerializeField] private float rotationSpeed = 5f;
        
        [Header("Vertical Settings")]
        [SerializeField] private float minHeight = 3f;
        [SerializeField] private float maxHeight = 10f;
        [SerializeField] private float heightDamping = 5f;
        
        [Header("Look Settings")]
        [SerializeField] private Vector3 lookOffset = new Vector3(0, 1.5f, 5);
        [SerializeField] private float lookAheadDistance = 10f;
        
        [Header("Shake Settings")]
        [SerializeField] private float shakeDuration = 0.3f;
        [SerializeField] private float shakeMagnitude = 0.5f;
        
        private Vector3 velocity;
        private float currentShakeTime;
        private Vector3 shakeOffset;
        private bool isShaking;
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            HandleShake();
            HandleFollow();
            HandleLookAt();
        }
        
        private void HandleFollow()
        {
            Vector3 targetPosition = target.position + offset;
            targetPosition.y = Mathf.Clamp(targetPosition.y, minHeight, maxHeight);
            
            // Smooth follow
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + shakeOffset, ref velocity, 1f / followSpeed);
        }
        
        private void HandleLookAt()
        {
            Vector3 lookTarget = target.position + lookOffset;
            lookTarget.z += lookAheadDistance;
            
            Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        private void HandleShake()
        {
            if (isShaking)
            {
                currentShakeTime -= Time.deltaTime;
                if (currentShakeTime <= 0)
                {
                    isShaking = false;
                    shakeOffset = Vector3.zero;
                }
                else
                {
                    float dampening = currentShakeTime / shakeDuration;
                    shakeOffset = new Vector3(
                        Random.Range(-1f, 1f) * shakeMagnitude * dampening,
                        Random.Range(-1f, 1f) * shakeMagnitude * dampening,
                        Random.Range(-1f, 1f) * shakeMagnitude * dampening
                    );
                }
            }
        }
        
        public void TriggerShake(float duration = -1, float magnitude = -1)
        {
            isShaking = true;
            currentShakeTime = duration > 0 ? duration : shakeDuration;
            // magnitude parameter could be used to scale shakeMagnitude temporarily
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
