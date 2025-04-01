using HexaClash.Game.Scripts.Data.EnumData;
using UnityEngine;

namespace HexaClash.Game.Scripts.Data.SOData.CharacterDatasSO
{
    [CreateAssetMenu(fileName = "AllyCharacterData", menuName = "Data/AllyCharacterData")]
    public class AllyCharacterDataSO : CharacterDataSO
    {
        #region Data

        [Header("Ally Character Data")]
        [SerializeField] private AllyCharacterType allyCharacterType;
        [SerializeField] private Sprite allyGraySprite;
        //[SerializeField] private Sprite hexagonElementSprite;

        #endregion

        #region Helpers

        public AllyCharacterType GetAllyCharacterType => allyCharacterType;
        public Sprite GetAllyGraySprite => allyGraySprite;

        #endregion
    }
}