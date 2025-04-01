using HexaClash.Game.Scripts.Data.ValueData;
using UnityEngine;

namespace HexaClash.Game.Scripts.Data.SOData
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "Data/LevelSettings")]
    public class LevelSettingsSO : ScriptableObject
    {
        #region Data

        [SerializeField] private HexagonBlockLevelData hexagonBlockLevel;
        [SerializeField] private EnemySpawnSettingsData[] enemySpawnSettings;

        #endregion

        #region Helpers

        public HexagonBlockLevelData HexagonBlockLevel => hexagonBlockLevel;
        public EnemySpawnSettingsData[] EnemySpawnSettings => enemySpawnSettings;

        #endregion
    }
}