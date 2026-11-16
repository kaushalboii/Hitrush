using UnityEngine;
using Hitrush.Player;

namespace Hitrush.Collectibles
{
    public class JetpackPowerUp : PowerUpBase
    {
        [SerializeField] private float flightHeight = 15f;
        [SerializeField] private float ascentSpeed = 10f;
        [SerializeField] private float coinAttractRadius = 20f;
        
        protected override void ApplyEffect(PlayerCollisionHandler player)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.StartCoroutine(JetpackRoutine(controller));
            }
            
            // Grant invincibility during jetpack
            player.GrantInvincibility(duration);
        }
        
        private System.Collections.IEnumerator JetpackRoutine(PlayerController controller)
        {
            float elapsed = 0f;
            float targetY = flightHeight;
            bool ascending = true;
            
            // Disable normal movement
            controller.enabled = false;
            
            // Enable flight movement
            var rigidbody = controller.GetComponent<Rigidbody>();
            if (rigidbody == null)
                rigidbody = controller.gameObject.AddComponent<Rigidbody>();
            
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
            
            while (elapsed < duration)
            {
                if (ascending)
                {
                    Vector3 pos = controller.transform.position;
                    pos.y = Mathf.MoveTowards(pos.y, targetY, ascentSpeed * Time.deltaTime);
                    controller.transform.position = pos;
                    
                    if (pos.y >= targetY - 0.5f)
                        ascending = false;
                }
                else
                {
                    // Maintain height, attract coins
                    Vector3 pos = controller.transform.position;
                    pos.y = targetY;
                    controller.transform.position = pos;
                    
                    // Auto-collect nearby coins
                    Collider[] coins = Physics.OverlapSphere(controller.transform.position, coinAttractRadius,
                        LayerMask.GetMask("Coin"));
                    foreach (var coin in coins)
                    {
                        var coinScript = coin.GetComponent<Coin>();
                        if (coinScript != null)
                            coinScript.Collect();
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Land
            rigidbody.useGravity = true;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            controller.enabled = true;
            
            OnExpire(controller.GetComponent<PlayerCollisionHandler>());
        }
        
        public override void OnExpire(PlayerCollisionHandler player)
        {
            // Jetpack effect ends
        }
    }
}
