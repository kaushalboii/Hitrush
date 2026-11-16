using UnityEngine;

namespace Hitrush.World
{
    public class TrackSegment : MonoBehaviour
    {
        [Header("Segment Settings")]
        [SerializeField] private float segmentLength = 35f;
        [SerializeField] private float laneWidth = 2.5f;
        
        [Header("Track Components")]
        [SerializeField] private GameObject trackMesh;
        [SerializeField] private GameObject leftBarrier;
        [SerializeField] private GameObject rightBarrier;
        [SerializeField] private GameObject[] environmentProps;
        
        [Header("Spawn Points")]
        [SerializeField] private Transform[] obstacleSpawnPoints;
        [SerializeField] private Transform[] coinSpawnPoints;
        [SerializeField] private Transform[] powerUpSpawnPoints;
        
        private bool isActive = false;
        private TrackSegmentSpawner spawner;
        
        public float Length => segmentLength;
        public bool IsActive => isActive;
        
        public void Initialize(TrackSegmentSpawner spawnerRef)
        {
            spawner = spawnerRef;
            isActive = true;
        }
        
        public void Deactivate()
        {
            isActive = false;
            if (spawner != null)
            {
                spawner.ReturnSegment(this);
            }
        }
        
        public Transform GetRandomObstacleSpawnPoint()
        {
            if (obstacleSpawnPoints.Length == 0) return transform;
            return obstacleSpawnPoints[Random.Range(0, obstacleSpawnPoints.Length)];
        }
        
        public Transform GetRandomCoinSpawnPoint()
        {
            if (coinSpawnPoints.Length == 0) return transform;
            return coinSpawnPoints[Random.Range(0, coinSpawnPoints.Length)];
        }
        
        public Transform GetRandomPowerUpSpawnPoint()
        {
            if (powerUpSpawnPoints.Length == 0) return transform;
            return powerUpSpawnPoints[Random.Range(0, powerUpSpawnPoints.Length)];
        }
        
        public void SetEnvironmentActive(bool active)
        {
            if (trackMesh != null) trackMesh.SetActive(active);
            if (leftBarrier != null) leftBarrier.SetActive(active);
            if (rightBarrier != null) rightBarrier.SetActive(active);
            
            foreach (var prop in environmentProps)
            {
                if (prop != null) prop.SetActive(active);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            // Check if player passed this segment
            if (other.CompareTag("Player") && isActive)
            {
                // Notify spawner to potentially spawn next segment
                if (spawner != null)
                {
                    spawner.OnPlayerPassedSegment(this);
                }
            }
        }
    }
}
