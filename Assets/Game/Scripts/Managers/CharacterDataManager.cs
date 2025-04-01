using System.Collections.Generic;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Data.EnumData;
using HexaClash.Game.Scripts.Data.SOData.CharacterDatasSO;
using UnityEngine;

namespace HexaClash.Game.Scripts.Managers
{
    public class CharacterDataManager : ManagerBase
    {
        #region Serialized Fields

        [SerializeField] private List<AllyCharacterDataSO> allyCharacterData;
        [SerializeField] private List<EnemyDataSO> enemyData;

        #endregion

        #region Override Methods

        public override void InitializeSystem()
        {
            InitializeSystemBody();
        }

        #endregion

        #region Public Methods

        public CharacterDataSO GetAllyCharacterData(AllyCharacterType allyCharacterType)
        {
            foreach (var data in allyCharacterData)
            {
                if (data.GetAllyCharacterType == allyCharacterType)
                {
                    return data;
                }
            }

            return null;
        }

        public EnemyDataSO GetEnemyData(EnemyType enemyType)
        {
            foreach (var data in enemyData)
            {
                if (data.enemyType == enemyType)
                {
                    return data;
                }
            }

            return null;
        }

        #endregion
    }
}