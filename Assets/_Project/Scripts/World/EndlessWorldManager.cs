using UnityEngine;
using Hitrush.Obstacles;
using Hitrush.Collectibles;
using Hitrush.Systems;
using Hitrush.Core;

namespace Hitrush.World
{
    public class EndlessWorldManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TrackSegmentSpawner segmentSpawner;
        [SerializeField] private ObstacleSpawner obstacleSpawner;
        [SerializeField] private CoinSpawner coinSpawner;
        [SerializeField] private PowerUpSpawner powerUpSpawner;
        
        [Header("Difficulty")]
        [SerializeField] private DifficultyManager difficultyManager;
        
        private Transform playerTransform;
        
        private void Awake()
        {
            // Find references if not assigned
            if (segmentSpawner == null) segmentSpawner = FindObjectOfType<TrackSegmentSpawner>();
            if (obstacleSpawner == null) obstacleSpawner = FindObjectOfType<ObstacleSpawner>();
            if (coinSpawner == null) coinSpawner = FindObjectOfType<CoinSpawner>();
            if (powerUpSpawner == null) powerUpSpawner = FindObjectOfType<PowerUpSpawner>();
            if (difficultyManager == null) difficultyManager = FindObjectOfType<DifficultyManager>();
        }
        
        public void Initialize(Transform player)
        {
            playerTransform = player;
            
            segmentSpawner?.Initialize(player, this);
            obstacleSpawner?.Initialize(this);
            coinSpawner?.Initialize(this);
            powerUpSpawner?.Initialize(this);
        }
        
        public void PopulateSegment(TrackSegment segment)
        {
            if (segment == null) return;
            
            float difficulty = difficultyManager != null ? difficultyManager.CurrentDifficulty : 1f;
            
            // Spawn obstacles
            obstacleSpawner?.SpawnOnSegment(segment, difficulty);
            
            // Spawn coins
            coinSpawner?.SpawnOnSegment(segment, difficulty);
            
            // Spawn power-ups (rare)
            powerUpSpawner?.SpawnOnSegment(segment, difficulty);
        }
        
        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;
            
            // Update difficulty based on distance
            if (difficultyManager != null && playerTransform != null)
            {
                difficultyManager.UpdateDifficulty(playerTransform.position.z);
            }
        }
    }
}
