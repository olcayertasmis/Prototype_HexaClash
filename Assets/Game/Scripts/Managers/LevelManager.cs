using System;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Data.SOData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HexaClash.Game.Scripts.Managers
{
    public class LevelManager : ManagerBase
    {
        [Header("Level Settings")]
        [SerializeField] private LevelSettingsSO[] levelSettings;

        private int _activeLevelIndex;
        private LevelSettingsSO _activeLevelSettings;

        public Action<LevelSettingsSO, int> OnLevelStarted;

        #region Override Methods

        public override void InitializeSystem()
        {
            InitializeSystemBody();

            _activeLevelIndex = PlayerPrefs.GetInt("Level", 0);
        }

        #endregion

        #region Unity Methods

        private void Start()
        {
            StartLevel();
        }

        #endregion

        private void StartLevel()
        {
            if (_activeLevelIndex < 0 || _activeLevelIndex >= levelSettings.Length)
            {
                Debug.LogError($"Invalid level index: {_activeLevelIndex}");
                return;
            }

            _activeLevelSettings = levelSettings[_activeLevelIndex];
            OnLevelStarted?.Invoke(_activeLevelSettings, _activeLevelIndex);
        }

        public LevelSettingsSO GetCurrentLevelSettings()
        {
            return _activeLevelSettings;
        }

        public void LoadNextLevel()
        {
            Debug.Log("Loading next level");

            int nextLevelIndex = _activeLevelIndex + 1;
            PlayerPrefs.SetInt("Level", nextLevelIndex);

            if (nextLevelIndex < levelSettings.Length)
            {
                var activeScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(activeScene.name);
            }
            else
            {
                Debug.Log("All levels completed!");
            }
        }
    }
}