using UnityEngine;
using Hitrush.Player;

namespace Hitrush.Collectibles
{
    public abstract class PowerUpBase : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private PowerUpType powerUpType;
        [SerializeField] private float duration = 10f;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private float bobHeight = 0.5f;
        [SerializeField] private float bobSpeed = 1.5f;
        
        [Header("Visual")]
        [SerializeField] private ParticleSystem collectParticles;
        [SerializeField] private AudioClip collectSound;
        
        private Vector3 startPosition;
        private bool isCollected = false;
        
        public PowerUpType Type => powerUpType;
        public float Duration => duration;
        
        private void Start()
        {
            startPosition = transform.position;
        }
        
        private void Update()
        {
            if (isCollected) return;
            
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
            
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, startPosition.y + bob, startPosition.z);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!isCollected && other.CompareTag("Player"))
            {
                var collisionHandler = other.GetComponent<PlayerCollisionHandler>();
                if (collisionHandler != null)
                {
                    Activate(collisionHandler);
                }
            }
        }
        
        public void Activate(PlayerCollisionHandler player)
        {
            if (isCollected) return;
            isCollected = true;
            
            // Visual/audio feedback
            if (collectParticles != null)
                Instantiate(collectParticles, transform.position, Quaternion.identity);
                
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            
            // Apply effect
            ApplyEffect(player);
            
            // Notify UI
            PowerUpManager.Instance?.ActivatePowerUp(this);
            
            // Return to pool or destroy
            var pool = GetComponentInParent<ObjectPool>();
            if (pool != null)
                pool.Return(gameObject);
            else
                Destroy(gameObject);
        }
        
        protected abstract void ApplyEffect(PlayerCollisionHandler player);
        
        public virtual void OnExpire(PlayerCollisionHandler player) { }
    }
}
