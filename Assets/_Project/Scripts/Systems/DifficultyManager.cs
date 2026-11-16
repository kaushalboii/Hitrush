using UnityEngine;

namespace Hitrush.Systems
{
    public class DifficultyManager : MonoBehaviour
    {
        public static DifficultyManager Instance { get; private set; }
        
        [Header("Speed Scaling")]
        [SerializeField] private float baseSpeed = 10f;
        [SerializeField] private float maxSpeed = 25f;
        [SerializeField] private float speedIncreasePer1000m = 0.5f;
        
        [Header("Spawn Rate Scaling")]
        [SerializeField] private float baseObstacleDensity = 1f;
        [SerializeField] private float maxObstacleDensity = 3f;
        [SerializeField] private float densityIncreasePer1000m = 0.2f;
        
        [Header("Coin Scaling")]
        [SerializeField] private float baseCoinDensity = 1f;
        [SerializeField] private float maxCoinDensity = 2f;
        [SerializeField] private float coinDensityIncreasePer1000m = 0.1f;
        
        public float CurrentDifficulty { get; private set; } = 1f;
        public float CurrentSpeed => Mathf.Min(baseSpeed + (CurrentDifficulty - 1f) * speedIncreasePer1000m * 10f, maxSpeed);
        public float ObstacleDensity => Mathf.Min(baseObstacleDensity + (CurrentDifficulty - 1f) * densityIncreasePer1000m, maxObstacleDensity);
        public float CoinDensity => Mathf.Min(baseCoinDensity + (CurrentDifficulty - 1f) * coinDensityIncreasePer1000m, maxCoinDensity);
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        public void UpdateDifficulty(float distance)
        {
            CurrentDifficulty = 1f + (distance / 1000f);
        }
        
        public void Reset()
        {
            CurrentDifficulty = 1f;
        }
    }
}
