using System;
using HexaClash.Game.Scripts.Data.SOData;
using UnityEngine;

namespace HexaClash.Game.Scripts.Data.ValueData
{
    [Serializable]
    public struct HexagonBlockData
    {
        #region Data

        [SerializeField] private HexagonElementDataSO[] hexagonData;
        [SerializeField] private int hexagonAmount;

        #endregion

        #region Helpers

        public HexagonElementDataSO[] GetHexagonData => hexagonData;
        public int GetHexagonAmount => hexagonAmount;

        #endregion
    }
}