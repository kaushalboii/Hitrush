using UnityEngine;
using Hitrush.Core;
using Hitrush.Collectibles;
using Hitrush.Obstacles;

namespace Hitrush.Player
{
    public class PlayerCollisionHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController playerController;
        
        [Header("Collision Settings")]
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask coinLayer;
        [SerializeField] private LayerMask powerUpLayer;
        
        [Header("Effects")]
        [SerializeField] private ParticleSystem deathParticles;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip coinSound;
        [SerializeField] private AudioClip powerUpSound;
        
        private bool hasShield = false;
        private bool isInvincible = false;
        private AudioSource audioSource;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;
            
            // Check for obstacle collision
            if (((1 << hit.gameObject.layer) & obstacleLayer) != 0)
            {
                HandleObstacleCollision(hit.gameObject);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;
            
            // Coin collection
            if (((1 << other.gameObject.layer) & coinLayer) != 0)
            {
                var coin = other.GetComponent<Coin>();
                if (coin != null)
                {
                    CollectCoin(coin);
                }
            }
            
            // Power-up collection
            if (((1 << other.gameObject.layer) & powerUpLayer) != 0)
            {
                var powerUp = other.GetComponent<PowerUpBase>();
                if (powerUp != null)
                {
                    CollectPowerUp(powerUp);
                }
            }
        }
        
        private void HandleObstacleCollision(GameObject obstacle)
        {
            if (isInvincible || hasShield)
            {
                if (hasShield)
                {
                    hasShield = false;
                    // Shield breaks - visual feedback
                    ObstacleBase obs = obstacle.GetComponent<ObstacleBase>();
                    if (obs != null)
                    {
                        obs.OnPlayerHitWithShield();
                    }
                    return; // Shield absorbs the hit
                }
                return; // Invincible (e.g., jetpack)
            }
            
            // Check if obstacle can be avoided by current action
            var obstacleBase = obstacle.GetComponent<ObstacleBase>();
            if (obstacleBase != null)
            {
                bool canAvoid = false;
                
                if (obstacleBase.AvoidanceType == ObstacleAvoidanceType.Jump && playerController.IsJumping())
                    canAvoid = true;
                else if (obstacleBase.AvoidanceType == ObstacleAvoidanceType.Slide && playerController.IsSliding())
                    canAvoid = true;
                else if (obstacleBase.AvoidanceType == ObstacleAvoidanceType.LaneChange)
                    canAvoid = true; // Player can change lanes
                
                if (canAvoid)
                {
                    // Player avoided it correctly
                    return;
                }
            }
            
            // Death
            Die();
        }
        
        private void CollectCoin(Coin coin)
        {
            coin.Collect();
            
            if (coinSound != null && audioSource != null)
                audioSource.PlayOneShot(coinSound);
                
            // Coin particle effect would be on the coin itself
        }
        
        private void CollectPowerUp(PowerUpBase powerUp)
        {
            powerUp.Activate(this);
            
            if (powerUpSound != null && audioSource != null)
                audioSource.PlayOneShot(powerUpSound);
        }
        
        public void GrantShield()
        {
            hasShield = true;
            // Visual indicator for shield
        }
        
        public void GrantInvincibility(float duration)
        {
            StartCoroutine(InvincibilityRoutine(duration));
        }
        
        private System.Collections.IEnumerator InvincibilityRoutine(float duration)
        {
            isInvincible = true;
            yield return new WaitForSeconds(duration);
            isInvincible = false;
        }
        
        private void Die()
        {
            if (deathParticles != null)
                Instantiate(deathParticles, transform.position, Quaternion.identity);
                
            if (deathSound != null && audioSource != null)
                audioSource.PlayOneShot(deathSound);
                
            playerController?.Die();
        }
        
        public bool HasShield() => hasShield;
        public bool IsInvincible() => isInvincible;
    }
}
