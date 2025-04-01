using System;
using HexaClash.Game.Scripts.Data.EnumData;
using UnityEngine;

namespace HexaClash.Game.Scripts.Data.ValueData
{
    [Serializable]
    public struct EnemySpawnSettingsData
    {
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private int spawnCount;

        public EnemyType EnemyType => enemyType;
        public int SpawnCount => spawnCount;
    }
}