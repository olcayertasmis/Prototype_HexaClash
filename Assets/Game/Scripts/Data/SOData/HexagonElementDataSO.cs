using HexaClash.Game.Scripts.Data.EnumData;
using HexaClash.Game.Scripts.Data.SOData.CharacterDatasSO;
using UnityEngine;

namespace HexaClash.Game.Scripts.Data.SOData
{
    [CreateAssetMenu(fileName = "HexagonElementData", menuName = "Data/HexagonElementData")]
    public class HexagonElementDataSO : ScriptableObject
    {
        #region Data

        [SerializeField] private HexagonType hexagonType;
        [SerializeField] private AllyCharacterDataSO allyCharacterDataSO;
        [SerializeField] private GameObject hexagonElementPrefab;

        #endregion

        #region Helpers

        public HexagonType HexagonType => hexagonType;
        public AllyCharacterDataSO GetAllyCharacterDataSO => allyCharacterDataSO;
        public GameObject GetHexagonElementPrefab => hexagonElementPrefab;

        #endregion
    }
}