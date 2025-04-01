using System;
using System.Linq;
using UnityEngine;

namespace HexaClash.Game.Scripts.Data.ValueData
{
    [Serializable]
    public struct HexagonBlockLevelData
    {
        [SerializeField] private HexagonBlockData[] hexagonBlockData;
        public HexagonBlockData[] GetHexagonBlockData => hexagonBlockData;
        public int MoveCount => hexagonBlockData.Count();
    }
}