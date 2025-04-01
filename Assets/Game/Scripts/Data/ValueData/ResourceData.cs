using HexaClash.Game.Scripts.Data.EnumData;
using UnityEngine;

namespace HexaClash.Game.Scripts.Data.ValueData
{
    [System.Serializable]
    public struct ResourceData
    {
        public ResourceType type;
        public Sprite icon;
        [Min(0)] public int amount;
    }
}