using UnityEngine;
using Hitrush.Collectibles;
using System.Collections.Generic;

namespace Hitrush.Systems
{
    public class PowerUpManager : MonoBehaviour
    {
        public static PowerUpManager Instance { get; private set; }
        
        private Dictionary<PowerUpType, PowerUpBase> activePowerUps = new Dictionary<PowerUpType, PowerUpBase>();
        private Dictionary<PowerUpType, float> powerUpEndTimes = new Dictionary<PowerUpType, float>();
        
        public Action<PowerUpType, float> OnPowerUpActivated;
        public Action<PowerUpType> OnPowerUpExpired;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Update()
        {
            var expired = new List<PowerUpType>();
            float now = Time.time;
            
            foreach (var kvp in powerUpEndTimes)
            {
                if (now >= kvp.Value)
                {
                    expired.Add(kvp.Key);
                }
            }
            
            foreach (var type in expired)
            {
                ExpirePowerUp(type);
            }
        }
        
        public void ActivatePowerUp(PowerUpBase powerUp)
        {
            PowerUpType type = powerUp.Type;
            float endTime = Time.time + powerUp.Duration;
            
            activePowerUps[type] = powerUp;
            powerUpEndTimes[type] = endTime;
            
            OnPowerUpActivated?.Invoke(type, powerUp.Duration);
        }
        
        private void ExpirePowerUp(PowerUpType type)
        {
            if (activePowerUps.TryGetValue(type, out var powerUp))
            {
                // Find player to call OnExpire
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    var collisionHandler = player.GetComponent<Hitrush.Player.PlayerCollisionHandler>();
                    if (collisionHandler != null)
                    {
                        powerUp.OnExpire(collisionHandler);
                    }
                }
            }
            
            activePowerUps.Remove(type);
            powerUpEndTimes.Remove(type);
            
            OnPowerUpExpired?.Invoke(type);
        }
        
        public bool IsPowerUpActive(PowerUpType type) => activePowerUps.ContainsKey(type);
        public float GetRemainingTime(PowerUpType type) 
        {
            if (powerUpEndTimes.TryGetValue(type, out float endTime))
                return Mathf.Max(0f, endTime - Time.time);
            return 0f;
        }
    }
}
