using UnityEngine;
using Hitrush.World;

namespace Hitrush.Collectibles
{
    public class Coin : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float bobHeight = 0.3f;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private Material glowMaterial;
        [SerializeField] private ParticleSystem collectParticles;
        
        [Header("Audio")]
        [SerializeField] private AudioClip collectSound;
        
        [Header("Value")]
        [SerializeField] private int coinValue = 1;
        
        private Vector3 startPosition;
        private bool isCollected = false;
        private float collectAnimationTime = 0f;
        
        private void Start()
        {
            startPosition = transform.position;
            
            // Apply glow material if available
            if (glowMaterial != null)
            {
                var renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    var mats = renderer.materials;
                    System.Array.Resize(ref mats, mats.Length + 1);
                    mats[mats.Length - 1] = glowMaterial;
                    renderer.materials = mats;
                }
            }
        }
        
        private void Update()
        {
            if (isCollected) return;
            
            // Rotate
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
            
            // Bob up/down
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, startPosition.y + bob, startPosition.z);
        }
        
        public void Collect()
        {
            if (isCollected) return;
            isCollected = true;
            
            // Particles
            if (collectParticles != null)
            {
                Instantiate(collectParticles, transform.position, Quaternion.identity);
            }
            
            // Sound
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }
            
            // Notify currency manager
            CurrencyManager.Instance?.AddCoins(coinValue);
            ScoreManager.Instance?.AddCoinScore(coinValue);
            
            // Return to pool or destroy
            var pool = GetComponentInParent<ObjectPool>();
            if (pool != null)
            {
                pool.Return(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!isCollected && other.CompareTag("Player"))
            {
                Collect();
            }
        }
    }
}
