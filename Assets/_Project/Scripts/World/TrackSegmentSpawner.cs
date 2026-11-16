using UnityEngine;
using System.Collections.Generic;

namespace Hitrush.World
{
    public class TrackSegmentSpawner : MonoBehaviour
    {
        [Header("Segment Settings")]
        [SerializeField] private GameObject trackSegmentPrefab;
        [SerializeField] private int initialSegments = 5;
        [SerializeField] private int maxActiveSegments = 10;
        [SerializeField] private float segmentLength = 35f;
        
        [Header("Generation")]
        [SerializeField] private float spawnAheadDistance = 100f;
        [SerializeField] private float recycleBehindDistance = 50f;
        
        // Pool
        private Queue<TrackSegment> availableSegments = new Queue<TrackSegment>();
        private List<TrackSegment> activeSegments = new List<TrackSegment>();
        
        // References
        private Transform playerTransform;
        private EndlessWorldManager worldManager;
        private float nextSpawnZ;
        
        private void Awake()
        {
            // Pre-warm pool
            for (int i = 0; i < initialSegments; i++)
            {
                CreatePooledSegment();
            }
        }
        
        public void Initialize(Transform player, EndlessWorldManager manager)
        {
            playerTransform = player;
            worldManager = manager;
            nextSpawnZ = 0f;
            
            // Spawn initial segments
            for (int i = 0; i < initialSegments; i++)
            {
                SpawnNextSegment();
            }
        }
        
        private void Update()
        {
            if (playerTransform == null || worldManager == null) return;
            
            // Check if we need to spawn more segments ahead
            float playerZ = playerTransform.position.z;
            if (playerZ + spawnAheadDistance > nextSpawnZ)
            {
                SpawnNextSegment();
            }
            
            // Recycle segments behind player
            RecycleOldSegments(playerZ);
        }
        
        private TrackSegment CreatePooledSegment()
        {
            GameObject go = Instantiate(trackSegmentPrefab);
            go.SetActive(false);
            var segment = go.GetComponent<TrackSegment>();
            if (segment != null)
            {
                segment.Initialize(this);
                availableSegments.Enqueue(segment);
            }
            return segment;
        }
        
        private void SpawnNextSegment()
        {
            TrackSegment segment;
            
            if (availableSegments.Count > 0)
            {
                segment = availableSegments.Dequeue();
                segment.gameObject.SetActive(true);
            }
            else
            {
                segment = CreatePooledSegment();
                segment.gameObject.SetActive(true);
            }
            
            // Position segment
            segment.transform.position = new Vector3(0, 0, nextSpawnZ);
            segment.SetEnvironmentActive(true);
            
            activeSegments.Add(segment);
            nextSpawnZ += segmentLength;
            
            // Notify world manager to populate this segment
            worldManager?.PopulateSegment(segment);
        }
        
        private void RecycleOldSegments(float playerZ)
        {
            for (int i = activeSegments.Count - 1; i >= 0; i--)
            {
                TrackSegment segment = activeSegments[i];
                if (segment.transform.position.z + segmentLength < playerZ - recycleBehindDistance)
                {
                    segment.SetEnvironmentActive(false);
                    segment.gameObject.SetActive(false);
                    activeSegments.RemoveAt(i);
                    availableSegments.Enqueue(segment);
                }
            }
        }
        
        public void ReturnSegment(TrackSegment segment)
        {
            if (activeSegments.Contains(segment))
            {
                activeSegments.Remove(segment);
            }
            segment.gameObject.SetActive(false);
            availableSegments.Enqueue(segment);
        }
        
        public void OnPlayerPassedSegment(TrackSegment segment)
        {
            // Could trigger events, increase difficulty, etc.
        }
    }
}
