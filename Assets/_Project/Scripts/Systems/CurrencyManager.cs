using System;
using UnityEngine;

namespace Hitrush.Systems
{
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }
        
        private int totalCoins = 0;
        private int runCoins = 0;
        
        public int TotalCoins => totalCoins;
        public int RunCoins => runCoins;
        
        public Action<int> OnTotalCoinsChanged;
        public Action<int> OnRunCoinsChanged;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        }
        
        public void AddCoins(int amount)
        {
            runCoins += amount;
            totalCoins += amount;
            
            OnRunCoinsChanged?.Invoke(runCoins);
            OnTotalCoinsChanged?.Invoke(totalCoins);
        }
        
        public void SpendCoins(int amount)
        {
            if (totalCoins >= amount)
            {
                totalCoins -= amount;
                PlayerPrefs.SetInt("TotalCoins", totalCoins);
                PlayerPrefs.Save();
                OnTotalCoinsChanged?.Invoke(totalCoins);
            }
        }
        
        public void OnRunEnd()
        {
            PlayerPrefs.SetInt("TotalCoins", totalCoins);
            PlayerPrefs.Save();
            
            runCoins = 0;
            OnRunCoinsChanged?.Invoke(runCoins);
        }
        
        public void ResetRunCoins()
        {
            runCoins = 0;
            OnRunCoinsChanged?.Invoke(runCoins);
        }
        
        public bool CanAfford(int amount) => totalCoins >= amount;
    }
}
