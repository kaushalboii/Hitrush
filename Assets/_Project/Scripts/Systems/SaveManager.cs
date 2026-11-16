using UnityEngine;

namespace Hitrush.Systems
{
    public static class SaveManager
    {
        private const string BEST_SCORE_KEY = "BestScore";
        private const string TOTAL_COINS_KEY = "TotalCoins";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string SFX_VOLUME_KEY = "SfxVolume";
        private const string VIBRATION_KEY = "VibrationEnabled";
        private const string SENSITIVITY_KEY = "ControlSensitivity";
        
        public static int BestScore
        {
            get => PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
            set => PlayerPrefs.SetInt(BEST_SCORE_KEY, value);
        }
        
        public static int TotalCoins
        {
            get => PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
            set => PlayerPrefs.SetInt(TOTAL_COINS_KEY, value);
        }
        
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.5f);
            set => PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        }
        
        public static float SfxVolume
        {
            get => PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.7f);
            set => PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        }
        
        public static bool VibrationEnabled
        {
            get => PlayerPrefs.GetInt(VIBRATION_KEY, 1) == 1;
            set => PlayerPrefs.SetInt(VIBRATION_KEY, value ? 1 : 0);
        }
        
        public static float ControlSensitivity
        {
            get => PlayerPrefs.GetFloat(SENSITIVITY_KEY, 1f);
            set => PlayerPrefs.SetFloat(SENSITIVITY_KEY, value);
        }
        
        public static void Save()
        {
            PlayerPrefs.Save();
        }
        
        public static void ResetAll()
        {
            PlayerPrefs.DeleteKey(BEST_SCORE_KEY);
            PlayerPrefs.DeleteKey(TOTAL_COINS_KEY);
            PlayerPrefs.DeleteKey(MUSIC_VOLUME_KEY);
            PlayerPrefs.DeleteKey(SFX_VOLUME_KEY);
            PlayerPrefs.DeleteKey(VIBRATION_KEY);
            PlayerPrefs.DeleteKey(SENSITIVITY_KEY);
            PlayerPrefs.Save();
        }
    }
}
