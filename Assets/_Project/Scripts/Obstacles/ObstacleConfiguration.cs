using UnityEngine;

namespace Hitrush.Obstacles
{
    [CreateAssetMenu(fileName = "ObstacleConfig", menuName = "Hitrush/Obstacle Configuration")]
    public class ObstacleConfiguration : ScriptableObject
    {
        [Header("Obstacle Prefab")]
        public GameObject prefab;
        
        [Header("Spawn Settings")]
        public int[] allowedLanes = { 0, 1, 2 };
        public float minSpacing = 15f;
        public float maxSpacing = 40f;
        public int minGroupSize = 1;
        public int maxGroupSize = 3;
        
        [Header("Difficulty")]
        [Range(0f, 1f)] public float spawnWeight = 1f;
        public float minDifficultyToSpawn = 0f;
        public float maxDifficultyToSpawn = 10f;
        
        [Header("Behavior")]
        public ObstacleAvoidanceType avoidanceType = ObstacleAvoidanceType.LaneChange;
        public bool canBeInCombo = true; // Can appear with other obstacles
    }
}
