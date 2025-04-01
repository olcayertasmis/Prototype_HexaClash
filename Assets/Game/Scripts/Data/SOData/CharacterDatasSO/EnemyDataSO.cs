using HexaClash.Game.Scripts.Data.EnumData;
using UnityEngine;

namespace HexaClash.Game.Scripts.Data.SOData.CharacterDatasSO
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Data/EnemyData")]
    public class EnemyDataSO : CharacterDataSO
    {
        [Header("Enemy Data ")]
        public EnemyType enemyType;
        public float onStartMoveSpeed;
    }
}