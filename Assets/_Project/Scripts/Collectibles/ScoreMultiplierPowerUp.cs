using Hitrush.Player;

namespace Hitrush.Collectibles
{
    public class ScoreMultiplierPowerUp : PowerUpBase
    {
        [SerializeField] private int multiplier = 2;
        
        protected override void ApplyEffect(PlayerCollisionHandler player)
        {
            ScoreManager.Instance?.SetMultiplier(multiplier, duration);
            
            // Start expiration
            player.StartCoroutine(MultiplierRoutine(player));
        }
        
        private System.Collections.IEnumerator MultiplierRoutine(PlayerCollisionHandler player)
        {
            yield return new WaitForSeconds(duration);
            OnExpire(player);
        }
        
        public override void OnExpire(PlayerCollisionHandler player)
        {
            ScoreManager.Instance?.SetMultiplier(1, 0f);
        }
    }
}
