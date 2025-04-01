using System;
using HexaClash.Game.Scripts.Data.EnumData;
using UnityEngine;

namespace HexaClash.Game.Scripts.Data.ValueData
{
    [Serializable]
    public struct TowerPartsData
    {
        public GameObject towerTopHole;
        public GameObject towerDoor;
        public GameObject towerWindow;
    }

    [Serializable]
    public class TowerTopHoleModelData
    {
        public Mesh meshFilter;
        public TowerTopHoleType towerTopHoleType;
    }

    [Serializable]
    public struct TowerPointData
    {
        public HexagonType hexagonType;
        public Sprite towerPointSprite;
    }
}