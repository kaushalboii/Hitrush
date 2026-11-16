using UnityEngine;

namespace Hitrush.Obstacles
{
    public class ObstacleBase : MonoBehaviour
    {
        [Header("Obstacle Config")]
        [SerializeField] private ObstacleAvoidanceType avoidanceType = ObstacleAvoidanceType.None;
        [SerializeField] private int[] blockedLanes = { 0, 1, 2 }; // Which lanes this occupies
        [SerializeField] private bool isMoving = false;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float moveRange = 2.5f;
        
        [Header("Visual")]
        [SerializeField] private GameObject warningIndicator;
        [SerializeField] private float warningTime = 1.5f;
        
        private Vector3 startPosition;
        private bool warningShown = false;
        
        public ObstacleAvoidanceType AvoidanceType => avoidanceType;
        public int[] BlockedLanes => blockedLanes;
        
        private void Start()
        {
            startPosition = transform.position;
            
            if (warningIndicator != null)
                warningIndicator.SetActive(false);
        }
        
        private void Update()
        {
            if (isMoving)
            {
                float offset = Mathf.Sin(Time.time * moveSpeed) * moveRange;
                transform.position = startPosition + new Vector3(offset, 0, 0);
            }
        }
        
        public void ShowWarning()
        {
            if (!warningShown && warningIndicator != null)
            {
                warningShown = true;
                warningIndicator.SetActive(true);
                Invoke(nameof(HideWarning), warningTime);
            }
        }
        
        private void HideWarning()
        {
            if (warningIndicator != null)
                warningIndicator.SetActive(false);
        }
        
        public void OnPlayerHitWithShield()
        {
            // Shield broke - destroy obstacle or disable
            if (warningIndicator != null)
                warningIndicator.SetActive(false);
            
            // Visual feedback for shield break
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.gray;
            }
            
            // Disable collision
            var collider = GetComponent<Collider>();
            if (collider != null)
                collider.enabled = false;
        }
        
        public bool IsLaneBlocked(int laneIndex)
        {
            foreach (int blocked in blockedLanes)
            {
                if (blocked == laneIndex) return true;
            }
            return false;
        }
    }
}
