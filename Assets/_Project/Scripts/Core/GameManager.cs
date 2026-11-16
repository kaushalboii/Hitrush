using UnityEngine;
using UnityEngine.SceneManagement;
using Hitrush.Core;

namespace Hitrush.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        public GameState CurrentState { get; private set; } = GameState.MainMenu;
        public Action<GameState> OnStateChanged;
        
        [Header("Scene Names")]
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string gameplayScene = "Gameplay";
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
            
            switch (newState)
            {
                case GameState.MainMenu:
                    SceneManager.LoadScene(mainMenuScene);
                    break;
                case GameState.Playing:
                    SceneManager.LoadScene(gameplayScene);
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 1f;
                    break;
            }
        }
        
        public void PauseGame() => SetState(GameState.Paused);
        public void ResumeGame() => SetState(GameState.Playing);
        public void GameOver() => SetState(GameState.GameOver);
        public void ReturnToMenu() => SetState(GameState.MainMenu);
        public void StartGame() => SetState(GameState.Playing);
    }
}
