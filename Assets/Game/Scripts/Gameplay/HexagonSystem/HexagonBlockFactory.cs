using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Data.SOData;
using HexaClash.Game.Scripts.Data.ValueData;
using UnityEngine;

namespace HexaClash.Game.Scripts.Gameplay.HexagonSystem
{
    public static class HexagonBlockFactory
    {
        private static int _hexagonBlockCounter;

        public static void CreateHexagonBlock(HexagonBlockSpawnSlotData spawnSlot, HexagonBlockData blockData, TowerPartsData towerPartsData)
        {
            _hexagonBlockCounter++;

            var position = spawnSlot.spawnTransform.position;
            GameObject hexagonBlockObject = new GameObject("HexagonBlock " + _hexagonBlockCounter)
            {
                transform = { position = new Vector3(position.x, Constants.HexagonBlockDefaultHeight, position.z) }
            };

            HexagonBlock hexagonBlock = hexagonBlockObject.AddComponent<HexagonBlock>();
            hexagonBlock.Initialize(spawnSlot, towerPartsData);

            for (int j = 0; j < blockData.GetHexagonAmount; j++)
            {
                int characterIndex = Random.Range(0, blockData.GetHexagonData.Length);
                HexagonElementDataSO hexagonElementData = blockData.GetHexagonData[characterIndex];

                GameObject hexagonElementObject = Object.Instantiate(hexagonElementData.GetHexagonElementPrefab, hexagonBlockObject.transform);
                HexagonElement hexagonElement = hexagonElementObject.GetComponent<HexagonElement>();
                hexagonElement.Initialize(hexagonElementData);

                _ = hexagonBlock.AddHexagonElement(hexagonElement);
            }

            //return hexagonBlock;
        }
    }
}