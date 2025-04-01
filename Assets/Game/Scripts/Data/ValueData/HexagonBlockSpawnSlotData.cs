using System;
using UnityEngine;

namespace HexaClash.Game.Scripts.Data.ValueData
{
    [Serializable]
    public class HexagonBlockSpawnSlotData
    {
        public Transform spawnTransform;
        [HideInInspector] public bool isAvailable;
    }
}
