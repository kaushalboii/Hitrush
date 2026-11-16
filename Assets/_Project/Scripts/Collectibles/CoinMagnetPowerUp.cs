using UnityEngine;
using Hitrush.Player;

namespace Hitrush.Collectibles
{
    public class CoinMagnetPowerUp : PowerUpBase
    {
        [SerializeField] private float magnetRadius = 10f;
        [SerializeField] private float pullSpeed = 15f;
        
        private PlayerController playerController;
        private Coroutine magnetRoutine;
        
        protected override void ApplyEffect(PlayerCollisionHandler player)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                magnetRoutine = playerController.StartCoroutine(MagnetRoutine(playerController));
            }
            
            // Visual feedback on player
            var magnetEffect = player.GetComponentInChildren<CoinMagnetEffect>();
            if (magnetEffect == null)
            {
                // Could add a magnet visual effect here
            }
        }
        
        private System.Collections.IEnumerator MagnetRoutine(PlayerController controller)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // Find nearby coins and pull them
                Collider[] coins = Physics.OverlapSphere(controller.transform.position, magnetRadius, 
                    LayerMask.GetMask("Coin"));
                
                foreach (var coin in coins)
                {
                    var coinScript = coin.GetComponent<Coin>();
                    if (coinScript != null && !coinScript.IsCollected())
                    {
                        Vector3 dir = (controller.transform.position - coin.transform.position).normalized;
                        coin.transform.position += dir * pullSpeed * Time.deltaTime;
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            OnExpire(playerController.GetComponent<PlayerCollisionHandler>());
        }
        
        public override void OnExpire(PlayerCollisionHandler player)
        {
            if (magnetRoutine != null && playerController != null)
            {
                playerController.StopCoroutine(magnetRoutine);
            }
        }
    }
}
