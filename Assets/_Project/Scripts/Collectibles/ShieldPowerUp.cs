using Hitrush.Player;

namespace Hitrush.Collectibles
{
    public class ShieldPowerUp : PowerUpBase
    {
        protected override void ApplyEffect(PlayerCollisionHandler player)
        {
            player.GrantShield();
            
            // Start expiration timer
            player.StartCoroutine(ShieldRoutine(player));
        }
        
        private System.Collections.IEnumerator ShieldRoutine(PlayerCollisionHandler player)
        {
            yield return new WaitForSeconds(duration);
            OnExpire(player);
        }
        
        public override void OnExpire(PlayerCollisionHandler player)
        {
            // Shield expires - player loses shield
            // The shield is consumed on hit, so this just handles timeout
            // Visual effect could be removed here
        }
    }
}
