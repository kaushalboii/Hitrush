using UnityEngine;
using System.Collections.Generic;

namespace Hitrush.Systems
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("Music")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private float musicVolume = 0.5f;
        
        [Header("SFX")]
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip slideSound;
        [SerializeField] private AudioClip coinSound;
        [SerializeField] private AudioClip powerUpSound;
        [SerializeField] private AudioClip collisionSound;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private float sfxVolume = 0.7f;
        
        private AudioSource musicSource;
        private Queue<AudioSource> sfxPool = new Queue<AudioSource>();
        private const int SFX_POOL_SIZE = 10;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Music source
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume * SaveManager.MusicVolume;
            
            // SFX pool
            for (int i = 0; i < SFX_POOL_SIZE; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.volume = sfxVolume * SaveManager.SfxVolume;
                sfxPool.Enqueue(source);
            }
            
            // Load saved volumes
            musicVolume = SaveManager.MusicVolume;
            sfxVolume = SaveManager.SfxVolume;
        }
        
        private void Start()
        {
            PlayMusic();
        }
        
        public void PlayMusic()
        {
            if (backgroundMusic != null && musicSource != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }
        
        public void StopMusic()
        {
            musicSource?.Stop();
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
                musicSource.volume = musicVolume * SaveManager.MusicVolume;
            SaveManager.MusicVolume = musicVolume;
        }
        
        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            SaveManager.SfxVolume = sfxVolume;
        }
        
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            
            AudioSource source = sfxPool.Count > 0 ? sfxPool.Dequeue() : gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = sfxVolume * SaveManager.SfxVolume;
            source.Play();
            
            StartCoroutine(ReturnToPool(source, clip.length));
        }
        
        public void PlayJump() => PlaySFX(jumpSound);
        public void PlaySlide() => PlaySFX(slideSound);
        public void PlayCoin() => PlaySFX(coinSound);
        public void PlayPowerUp() => PlaySFX(powerUpSound);
        public void PlayCollision() => PlaySFX(collisionSound);
        public void PlayButtonClick() => PlaySFX(buttonClickSound);
        
        private System.Collections.IEnumerator ReturnToPool(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);
            source.Stop();
            sfxPool.Enqueue(source);
        }
    }
}
