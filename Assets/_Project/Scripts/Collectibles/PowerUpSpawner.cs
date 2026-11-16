using UnityEngine;
using System.Collections.Generic;
using Hitrush.World;

namespace Hitrush.Collectibles
{
    public class PowerUpSpawner : MonoBehaviour
    {
        [Header("Power-Up Prefabs")]
        [SerializeField] private GameObject coinMagnetPrefab;
        [SerializeField] private GameObject shieldPrefab;
        [SerializeField] private GameObject jetpackPrefab;
        [SerializeField] private GameObject multiplierPrefab;
        
        [Header("Pools")]
        [SerializeField] private ObjectPool coinMagnetPool;
        [SerializeField] private ObjectPool shieldPool;
        [SerializeField] private ObjectPool jetpackPool;
        [SerializeField] private ObjectPool multiplierPool;
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnChance = 0.08f; // 8% per segment
        [SerializeField] private float minDifficultyForPowerUp = 2f;
        
        private EndlessWorldManager worldManager;
        private Dictionary<PowerUpType, ObjectPool> pools = new Dictionary<PowerUpType, ObjectPool>();
        
        public void Initialize(EndlessWorldManager manager)
        {
            worldManager = manager;
            
            if (coinMagnetPool != null) pools[PowerUpType.CoinMagnet] = coinMagnetPool;
            if (shieldPool != null) pools[PowerUpType.Shield] = shieldPool;
            if (jetpackPool != null) pools[PowerUpType.Jetpack] = jetpackPool;
            if (multiplierPool != null) pools[PowerUpType.ScoreMultiplier] = multiplierPool;
        }
        
        public void SpawnOnSegment(TrackSegment segment, float difficulty)
        {
            if (difficulty < minDifficultyForPowerUp) return;
            if (Random.value > spawnChance) return;
            if (pools.Count == 0) return;
            
            // Pick random power-up
            var powerUpTypes = new List<PowerUpType>(pools.Keys);
            PowerUpType type = powerUpTypes[Random.Range(0, powerUpTypes.Count)];
            
            // Pick random lane
            int lane = Random.Range(0, 3);
            float x = lane == 0 ? -2.5f : (lane == 1 ? 0f : 2.5f);
            float z = segment.transform.position.z + Random.Range(10f, segment.Length - 10f);
            float y = 1.5f;
            
            SpawnPowerUp(type, new Vector3(x, y, z));
        }
        
        private void SpawnPowerUp(PowerUpType type, Vector3 position)
        {
            if (!pools.ContainsKey(type)) return;
            
            GameObject powerUp = pools[type].Get();
            if (powerUp != null)
            {
                powerUp.transform.position = position;
                powerUp.transform.rotation = Quaternion.identity;
            }
        }
    }
}
