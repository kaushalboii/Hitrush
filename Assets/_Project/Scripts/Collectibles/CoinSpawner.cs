using UnityEngine;
using System.Collections.Generic;
using Hitrush.World;
using Hitrush.Systems;

namespace Hitrush.Collectibles
{
    public class CoinSpawner : MonoBehaviour
    {
        [Header("Coin Prefab")]
        [SerializeField] private GameObject coinPrefab;
        
        [Header("Pool")]
        [SerializeField] private ObjectPool coinPool;
        
        [Header("Patterns")]
        [SerializeField] private CoinPattern[] patterns;
        
        [Header("Spawn Settings")]
        [SerializeField] private float coinsPerSegment = 15f;
        [SerializeField] private float patternChance = 0.6f;
        
        private EndlessWorldManager worldManager;
        
        public void Initialize(EndlessWorldManager manager)
        {
            worldManager = manager;
        }
        
        public void SpawnOnSegment(TrackSegment segment, float difficulty)
        {
            if (coinPool == null || coinPrefab == null) return;
            
            // Decide: pattern or random scatter
            if (patterns.Length > 0 && Random.value < patternChance)
            {
                SpawnPattern(segment);
            }
            else
            {
                SpawnRandom(segment);
            }
        }
        
        private void SpawnPattern(TrackSegment segment)
        {
            CoinPattern pattern = patterns[Random.Range(0, patterns.Length)];
            
            // Pattern defines relative positions
            foreach (Vector3 localPos in pattern.GetCoinPositions())
            {
                Vector3 worldPos = segment.transform.position + localPos;
                SpawnCoinAt(worldPos);
            }
        }
        
        private void SpawnRandom(TrackSegment segment)
        {
            int coinCount = Mathf.RoundToInt(coinsPerSegment * Random.Range(0.7f, 1.3f));
            
            for (int i = 0; i < coinCount; i++)
            {
                int lane = Random.Range(0, 3);
                float x = lane == 0 ? -2.5f : (lane == 1 ? 0f : 2.5f);
                float z = segment.transform.position.z + Random.Range(5f, segment.Length - 5f);
                float y = 1f + Random.Range(-0.5f, 1f);
                
                SpawnCoinAt(new Vector3(x, y, z));
            }
        }
        
        private void SpawnCoinAt(Vector3 position)
        {
            GameObject coin = coinPool.Get();
            if (coin != null)
            {
                coin.transform.position = position;
                coin.transform.rotation = Quaternion.identity;
                
                var coinScript = coin.GetComponent<Coin>();
                if (coinScript != null)
                {
                    // Reset coin state
                    coinScript.ResetCoin();
                }
            }
        }
    }
    
    [System.Serializable]
    public class CoinPattern
    {
        [SerializeField] private string patternName;
        [SerializeField] private Vector3[] relativePositions;
        
        public Vector3[] GetCoinPositions() => relativePositions;
    }
}
