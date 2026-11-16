using UnityEngine;
using System.Collections.Generic;
using Hitrush.World;

namespace Hitrush.Obstacles
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("Configurations")]
        [SerializeField] private List<ObstacleConfiguration> obstacleConfigs = new List<ObstacleConfiguration>();
        
        [Header("Pool")]
        [SerializeField] private ObjectPool obstaclePool;
        
        private EndlessWorldManager worldManager;
        private Dictionary<int, List<ObstacleConfiguration>> configsByLane = new Dictionary<int, List<ObstacleConfiguration>>();
        
        public void Initialize(EndlessWorldManager manager)
        {
            worldManager = manager;
            BuildLaneLookup();
        }
        
        private void BuildLaneLookup()
        {
            configsByLane.Clear();
            for (int i = 0; i < 3; i++)
            {
                configsByLane[i] = new List<ObstacleConfiguration>();
            }
            
            foreach (var config in obstacleConfigs)
            {
                foreach (int lane in config.allowedLanes)
                {
                    if (lane >= 0 && lane < 3)
                        configsByLane[lane].Add(config);
                }
            }
        }
        
        public void SpawnOnSegment(TrackSegment segment, float difficulty)
        {
            if (obstaclePool == null || obstacleConfigs.Count == 0) return;
            
            // Determine how many obstacles to spawn
            int obstacleCount = Random.Range(1, 4);
            
            // Track which lanes are blocked at which Z positions
            Dictionary<int, List<float>> laneBlockedZ = new Dictionary<int, List<float>>();
            for (int i = 0; i < 3; i++) laneBlockedZ[i] = new List<float>();
            
            for (int i = 0; i < obstacleCount; i++)
            {
                // Pick a random lane
                int lane = Random.Range(0, 3);
                
                // Get valid configs for this lane at current difficulty
                var validConfigs = GetValidConfigsForLane(lane, difficulty);
                if (validConfigs.Count == 0) continue;
                
                // Weighted random selection
                ObstacleConfiguration config = WeightedRandom(validConfigs);
                if (config == null) continue;
                
                // Check spacing
                float spawnZ = segment.transform.position.z + Random.Range(config.minSpacing, config.maxSpacing);
                
                if (!CanSpawnAt(lane, spawnZ, config.minSpacing, laneBlockedZ))
                    continue;
                
                // Spawn
                Vector3 spawnPos = new Vector3(
                    lane == 0 ? -2.5f : (lane == 1 ? 0f : 2.5f),
                    0.5f,
                    spawnZ
                );
                
                GameObject obstacle = obstaclePool.Get();
                if (obstacle != null)
                {
                    obstacle.transform.position = spawnPos;
                    obstacle.transform.rotation = Quaternion.identity;
                    
                    var obstacleBase = obstacle.GetComponent<ObstacleBase>();
                    if (obstacleBase != null)
                    {
                        // Apply config
                        System.Reflection.FieldInfo[] fields = typeof(ObstacleConfiguration).GetFields();
                        foreach (var field in fields)
                        {
                            var obstacleField = typeof(ObstacleBase).GetField(field.Name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (obstacleField != null)
                                obstacleField.SetValue(obstacleBase, field.GetValue(config));
                        }
                    }
                    
                    laneBlockedZ[lane].Add(spawnZ);
                }
            }
        }
        
        private List<ObstacleConfiguration> GetValidConfigsForLane(int lane, float difficulty)
        {
            var result = new List<ObstacleConfiguration>();
            
            if (configsByLane.ContainsKey(lane))
            {
                foreach (var config in configsByLane[lane])
                {
                    if (difficulty >= config.minDifficultyToSpawn && difficulty <= config.maxDifficultyToSpawn)
                    {
                        result.Add(config);
                    }
                }
            }
            
            return result;
        }
        
        private bool CanSpawnAt(int lane, float spawnZ, float minSpacing, Dictionary<int, List<float>> laneBlockedZ)
        {
            foreach (float existingZ in laneBlockedZ[lane])
            {
                if (Mathf.Abs(spawnZ - existingZ) < minSpacing)
                    return false;
            }
            
            // Check other lanes for combo restrictions
            // (simplified - could check for impossible combos)
            
            return true;
        }
        
        private ObstacleConfiguration WeightedRandom(List<ObstacleConfiguration> configs)
        {
            float totalWeight = 0f;
            foreach (var c in configs) totalWeight += c.spawnWeight;
            
            float roll = Random.Range(0f, totalWeight);
            float current = 0f;
            
            foreach (var c in configs)
            {
                current += c.spawnWeight;
                if (roll <= current) return c;
            }
            
            return configs[configs.Count - 1];
        }
    }
}
