using System.Collections;
using System.Collections.Generic;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace HexaClash.Game.Scripts.Gameplay.HexagonSystem
{
    public class HexagonArea : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private MeshRenderer meshRenderer;

        [Header("Locked Area Settings")]
        [SerializeField] private Material[] lockMaterials;
        [SerializeField] private TextMeshProUGUI unlockRequirementAmountText;

        #endregion

        #region Private Fields

        [Header("Controls")]
        private int _currentUnlockRequirement;
        private HexagonArea _lockedNeighbor;

        [Header("Data")]
        private List<HexagonArea> _neighbors;

        #endregion

        #region Props

        public HexagonBlock HexagonBlock { get; private set; }
        public bool IsLocked { get; private set; }

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            var battleManager = GameManager.Instance.GetSystem<BattleManager>();
            if (!battleManager) return;

            battleManager.OnBattleStart -= OnBattleStart;
            battleManager.OnBattleEnd -= OnBattleEnd;
        }

        #endregion

        #region Public Methods

        public void Initialize(bool isLocked = false)
        {
            IsLocked = isLocked;

            if (IsLocked)
            {
                _currentUnlockRequirement = Constants.HexagonAreaUnlockRequirement;

                ApplyLockedMaterials();

                UpdateUnlockUI();
            }
            else DestroyUnlockUI();


            SubscribeToBattleEvents();
        }

        public void SetNeighbors(List<HexagonArea> newNeighbors)
        {
            _neighbors = newNeighbors;

            if (IsLocked) return;

            foreach (var neighbor in _neighbors)
            {
                if (neighbor.IsLocked)
                {
                    _lockedNeighbor = neighbor;
                    break;
                }
            }
        }

        public void PlaceHexagonBlock(HexagonBlock newHexagonBlock)
        {
            if (HexagonBlock)
            {
                Debug.LogError("This area is already occupied!");
                return;
            }

            HexagonBlock = newHexagonBlock;
            newHexagonBlock.transform.position = transform.position;
            HexagonBlock.BlockOnNullInBattle += BlockOnNullInBattle;
            newHexagonBlock.OnPlaced();

            CheckForPotentialMerges(newHexagonBlock);
        }

        public IEnumerator ProcessMerge(HexagonBlock sourceBlock)
        {
            if (!HexagonBlock || !sourceBlock) yield break;

            Debug.Log($"Merged {HexagonBlock.TopElement.AllyCharacterType} blocks!");

            if (_lockedNeighbor) _lockedNeighbor.UnlockArea();

            while (CanMerge(sourceBlock, HexagonBlock))
            {
                HexagonElement topElement = null;
                yield return StartCoroutine(sourceBlock.RemoveTopElementCoroutine((removedElement) => { topElement = removedElement; }));
                if (!topElement) break;

                yield return HexagonBlock.AddHexagonElement(topElement, true);

                if (HexagonBlock.GetElements().Count >= Constants.MaxHexagonElementsPerBlock) break;
            }

            if (sourceBlock.GetElements().Count == 0) Destroy(sourceBlock.gameObject); // Source blok bossa destroy

            IncreaseBlockPower(HexagonBlock);
        }

        #endregion

        #region Private Methods

        private void CheckForPotentialMerges(HexagonBlock newHexagonBlock)
        {
            foreach (var neighbor in _neighbors)
            {
                if (CanMerge(newHexagonBlock, neighbor.HexagonBlock)) GameManager.Instance.GetSystem<MergeManager>().RequestMerge(this, neighbor.HexagonBlock);
            }
        }

        private bool CanMerge(HexagonBlock sourceBlock, HexagonBlock targetBlock)
        {
            return sourceBlock && targetBlock && sourceBlock.TopElement && targetBlock.TopElement && sourceBlock.TopElement.AllyCharacterType == targetBlock.TopElement.AllyCharacterType && sourceBlock.GetElements().Count < Constants.MaxHexagonElementsPerBlock;
        }

        private void IncreaseBlockPower(HexagonBlock block)
        {
            Debug.Log($"Block power increased: {block.TopElement.AllyCharacterType}");
        }

        #endregion

        #region Action Subscribe Methods

        private void SubscribeToBattleEvents()
        {
            var battleManager = GameManager.Instance.GetSystem<BattleManager>();
            if (!battleManager) return;

            battleManager.OnBattleStart += OnBattleStart;
            battleManager.OnBattleEnd += OnBattleEnd;
        }

        private void BlockOnNullInBattle()
        {
            if (!spriteRenderer.gameObject.activeSelf) spriteRenderer.gameObject.SetActive(true);
            spriteRenderer.sprite = HexagonBlock.TopElement.HexagonElementData.GetAllyCharacterDataSO.GetAllyGraySprite;
        }

        private void OnBattleStart()
        {
            if (!HexagonBlock) meshRenderer.enabled = false;
            if (IsLocked) unlockRequirementAmountText.transform.gameObject.SetActive(false);
        }

        private void OnBattleEnd()
        {
            if (IsLocked) unlockRequirementAmountText.transform.gameObject.SetActive(true);
            if (meshRenderer.enabled == false) meshRenderer.enabled = true;
        }

        #endregion

        #region Lock Area Methods

        private void UnlockArea()
        {
            if (!IsLocked) return;

            _currentUnlockRequirement--;
            UpdateUnlockUI();

            var meshRendererMaterials = meshRenderer.materials;
            if (_currentUnlockRequirement <= 0)
            {
                IsLocked = false;

                RemoveLockMaterials();
                DestroyUnlockUI();

                Debug.Log("Area unlocked!");
            }
            else UpdateLockMaterials(meshRendererMaterials);
        }

        private void RemoveLockMaterials()
        {
            var materials = meshRenderer.materials;
            if (materials.Length <= 1) return;

            var baseMaterial = materials[0];
            meshRenderer.materials = new[] { baseMaterial };
        }

        private void UpdateLockMaterials(Material[] meshRendererMaterials)
        {
            switch (_currentUnlockRequirement)
            {
                case 4:
                    Extensions.RemoveAt(ref meshRendererMaterials, 1);
                    break;
                case 3:
                    Extensions.RemoveAt(ref meshRendererMaterials, 2);
                    break;
                case 0:
                    Extensions.RemoveAt(ref meshRendererMaterials, 3);
                    Destroy(unlockRequirementAmountText.transform.parent.gameObject);
                    break;
            }

            meshRenderer.materials = meshRendererMaterials;
        }

        private void ApplyLockedMaterials()
        {
            var materials = new List<Material> { meshRenderer.material };
            materials.AddRange(lockMaterials);
            meshRenderer.materials = materials.ToArray();
        }

        private void UpdateUnlockUI()
        {
            unlockRequirementAmountText.text = _currentUnlockRequirement.ToString();
            if (!unlockRequirementAmountText.transform.parent.gameObject.activeSelf) unlockRequirementAmountText.transform.parent.gameObject.SetActive(true);
        }

        private void DestroyUnlockUI()
        {
            Destroy(unlockRequirementAmountText.transform.parent.gameObject);
            lockMaterials = null;
        }

        #endregion
    }
}