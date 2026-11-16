using UnityEngine;
using System;

namespace Hitrush.Systems
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }
        
        [Header("Scoring")]
        [SerializeField] private int scorePerMeter = 1;
        [SerializeField] private int scorePerCoin = 50;
        
        private int currentScore = 0;
        private int bestScore = 0;
        private int coinScore = 0;
        private int distanceScore = 0;
        private int multiplier = 1;
        private float multiplierEndTime = 0f;
        
        public int CurrentScore => currentScore;
        public int BestScore => bestScore;
        public int Multiplier => multiplier;
        
        public Action<int> OnScoreChanged;
        public Action<int> OnBestScoreChanged;
        public Action<int> OnMultiplierChanged;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            bestScore = PlayerPrefs.GetInt("BestScore", 0);
        }
        
        private void Update()
        {
            if (multiplier > 1 && Time.time > multiplierEndTime)
            {
                SetMultiplier(1, 0f);
            }
        }
        
        public void AddDistanceScore(float distance)
        {
            int newDistanceScore = Mathf.FloorToInt(distance * scorePerMeter * multiplier);
            if (newDistanceScore > distanceScore)
            {
                int delta = newDistanceScore - distanceScore;
                distanceScore = newDistanceScore;
                currentScore += delta;
                OnScoreChanged?.Invoke(currentScore);
            }
        }
        
        public void AddCoinScore(int coins)
        {
            int delta = coins * scorePerCoin * multiplier;
            coinScore += delta;
            currentScore += delta;
            OnScoreChanged?.Invoke(currentScore);
        }
        
        public void SetMultiplier(int newMultiplier, float duration)
        {
            multiplier = newMultiplier;
            multiplierEndTime = duration > 0 ? Time.time + duration : 0f;
            OnMultiplierChanged?.Invoke(multiplier);
        }
        
        public void OnRunEnd()
        {
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                PlayerPrefs.SetInt("BestScore", bestScore);
                PlayerPrefs.Save();
                OnBestScoreChanged?.Invoke(bestScore);
            }
            
            currentScore = 0;
            distanceScore = 0;
            coinScore = 0;
            OnScoreChanged?.Invoke(currentScore);
        }
        
        public void ResetRunScore()
        {
            currentScore = 0;
            distanceScore = 0;
            coinScore = 0;
            OnScoreChanged?.Invoke(currentScore);
        }
    }
}
