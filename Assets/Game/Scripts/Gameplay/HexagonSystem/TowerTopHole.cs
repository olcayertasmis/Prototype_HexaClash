using System;
using System.Collections;
using System.Linq;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Data.EnumData;
using HexaClash.Game.Scripts.Data.ValueData;
using UnityEngine;
using UnityEngine.UI;

namespace HexaClash.Game.Scripts.Gameplay.HexagonSystem
{
    public class TowerTopHole : MonoBehaviour
    {
        #region SerializeField Fields

        [Header("Component References")]
        [SerializeField] private MeshFilter towerTopHoleMeshFilter;

        [Header("UI References")]
        [SerializeField] private GameObject spawnSpeed;
        [SerializeField] private GameObject maxText;

        [Header("Data")]
        [SerializeField] private Image[] towerPointImages;
        [SerializeField] private TowerPointData[] towerPointData;
        [SerializeField] private TowerTopHoleModelData[] towerTopHoleModelData;
        private TowerTopHoleType _currentHoleType = TowerTopHoleType.Null;
        private HexagonType _currentHexagonType;

        #endregion

        #region Public Methods

        public void OnTowerUpdate(int hexagonBlockStackCount, HexagonType hexagonType)
        {
            if (hexagonBlockStackCount < Constants.ElementsRequiredForTower) return;

            var newHoleType = GetTowerTopHoleType(hexagonBlockStackCount);

            if (!Equals(_currentHexagonType, hexagonType)) _currentHexagonType = hexagonType;

            if (_currentHoleType != newHoleType)
            {
                _currentHoleType = newHoleType;

                UpdateTowerVisuals();

                StartCoroutine(ShowSpawnSpeedEffect());
            }

            UpdateMaxTextVisibility();
        }

        #endregion

        #region Private Methods

        private TowerTopHoleType GetTowerTopHoleType(int hexagonBlockStackCount)
        {
            return hexagonBlockStackCount switch
            {
                >= Constants.MaxHexagonElementsPerBlock => TowerTopHoleType.Max,
                >= Constants.ElementsRequiredForMediumTower => TowerTopHoleType.Medium,
                >= Constants.ElementsRequiredForTower => TowerTopHoleType.First,
                _ => TowerTopHoleType.Null
            };
        }

        private void UpdateTowerVisuals()
        {
            UpdateMeshFilter();
            UpdateTowerPointSprites();
        }

        private void UpdateMeshFilter()
        {
            var modelData = Array.Find(towerTopHoleModelData, x => x.towerTopHoleType == _currentHoleType);
            if (modelData != null && towerTopHoleMeshFilter) towerTopHoleMeshFilter.mesh = modelData.meshFilter;
        }

        private void UpdateTowerPointSprites()
        {
            var pointSprite = GetSpriteForHexagonType(_currentHexagonType);
            if (!pointSprite) return;

            for (int i = 0; i < (int)_currentHoleType; i++)
            {
                if (i < towerPointImages.Length) towerPointImages[i].sprite = pointSprite;
            }
        }

        private Sprite GetSpriteForHexagonType(HexagonType type) => towerPointData.FirstOrDefault(x => x.hexagonType == type).towerPointSprite;

        /*private Image GetImageForHoleType(TowerTopHoleType type)
        {
            return type switch
            {
                TowerTopHoleType.First => towerPointImages[0],
                TowerTopHoleType.Medium => towerPointImages[1],
                TowerTopHoleType.Max => towerPointImages[2],
                _ => null
            };
        }*/

        private IEnumerator ShowSpawnSpeedEffect()
        {
            if (!spawnSpeed) yield break;

            spawnSpeed.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            spawnSpeed.SetActive(false);
        }

        private void UpdateMaxTextVisibility()
        {
            if (!maxText.activeSelf && _currentHoleType == TowerTopHoleType.Max) maxText.SetActive(true);
        }

        #endregion
    }
}