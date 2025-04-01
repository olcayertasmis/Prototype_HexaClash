using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Data.ValueData;
using HexaClash.Game.Scripts.Managers;
using UnityEngine;

namespace HexaClash.Game.Scripts.Gameplay.HexagonSystem
{
    public class HexagonBlock : MonoBehaviour
    {
        #region Props

        public HexagonBlockSpawnSlotData HexagonBlockSpawnSlot { get; private set; }
        public bool IsMerging { get; private set; }
        public bool IsPlaced { get; private set; }

        #endregion

        #region Private Fields

        [Header("Data")]
        private Vector3 _initialPosition;
        private readonly Stack<HexagonElement> _hexagonStack = new();
        private Tween _elementMoveTween;

        [Header("Components")]
        private BoxCollider _boxCollider;

        [Header("Tower Parts")]
        private TowerTopHole _towerTopHole;
        private GameObject _towerDoor;
        private GameObject _towerWindow1;
        private GameObject _towerWindow2;

        #endregion

        #region Public Fields

        [Header("Actions")]
        public Action BlockOnNullInBattle;

        #endregion

        #region Helpers

        public HexagonElement TopElement => _hexagonStack.Count > 0 ? _hexagonStack.Peek() : null;

        public List<HexagonElement> GetElements() => new(_hexagonStack);

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _boxCollider = GetComponent<Collider>() as BoxCollider ?? gameObject.AddComponent<BoxCollider>();
            _boxCollider.size = Constants.HexagonElementColliderSize;
        }

        private void Start() => _initialPosition = transform.position;
        private void OnDestroy() => UnsubscribeActions();

        #endregion

        #region Public Methods

        public void Initialize(HexagonBlockSpawnSlotData spawnSlot, TowerPartsData towerPartsData)
        {
            HexagonBlockSpawnSlot = spawnSlot;

            InitializeTowerComponents(towerPartsData);
        }

        public void OnDragStart()
        {
            if (!IsPlaced) SetColliderEnabled(false);
        }

        public void OnDragEnd()
        {
            if (!IsPlaced) SetColliderEnabled(true);
        }

        public Task AddHexagonElement(HexagonElement hexagonElement, bool animate = false)
        {
            if (_hexagonStack.Count >= Constants.MaxHexagonElementsPerBlock)
            {
                Debug.LogWarning("HexagonBlock has reached its maximum capacity!");
                return Task.CompletedTask;
            }

            var yOffset = _hexagonStack.Count * (Constants.HexagonElementColliderSize.y + Constants.HexagonElementSpacing);
            var targetPosition = new Vector3(0, yOffset, 0);

            hexagonElement.transform.SetParent(transform);

            if (animate) MoveElementWithAnimation(hexagonElement, targetPosition);
            else hexagonElement.transform.localPosition = targetPosition;

            if (_hexagonStack.Count > 0) _hexagonStack.Peek().SetCanvasActive(false); // onceki top element'in sprite'ini pasif yap

            _hexagonStack.Push(hexagonElement);
            hexagonElement.SetCanvasActive(true);

            UpdateColliderSize();
            if (!animate) UpdateTowerComponents();

            if (_hexagonStack.Count == Constants.ElementsRequiredForTower) ConvertToTower();
            return Task.CompletedTask;
        }

        public IEnumerator RemoveTopElementCoroutine(Action<HexagonElement> onComplete, bool useAnimation = false)
        {
            if (_hexagonStack.Count == 0)
            {
                onComplete?.Invoke(null);
                yield break;
            }

            HexagonElement topElement = _hexagonStack.Pop();
            topElement.SetCanvasActive(false);

            if (_hexagonStack.Count > 0) _hexagonStack.Peek().SetCanvasActive(true);

            UpdateColliderSize();
            if (_hexagonStack.Count < Constants.ElementsRequiredForTower) ConvertToDefaultBlock();
            UpdateTowerComponents();

            onComplete?.Invoke(topElement);

            if (useAnimation)
            {
                yield return transform.DOScale(transform.localScale * 0.9f, 0.1f).WaitForCompletion();
                yield return transform.DOScale(Vector3.one, 0.1f).WaitForCompletion();
                topElement.gameObject.SetActive(false);

                if (_hexagonStack.Count == 1) BlockOnNullInBattle?.Invoke();
            }
        }

        public void ResetPosition() => transform.position = _initialPosition;

        public void OnPlaced()
        {
            IsPlaced = true;

            GameManager.Instance.GetSystem<HexagonBlockSpawnManager>()?.OnBlockPlaced(this);
        }

        public void StartMerge() => IsMerging = true;
        public void EndMerge() => IsMerging = false;

        #endregion

        #region Private Methods

        private void InitializeTowerComponents(TowerPartsData towerPartsData)
        {
            _towerTopHole = Instantiate(towerPartsData.towerTopHole, transform).GetComponent<TowerTopHole>();
            _towerTopHole.transform.localPosition = new Vector3(0, Constants.HexagonBlockDefaultHeight, 0);

            _towerDoor = Instantiate(towerPartsData.towerDoor, transform);
            _towerDoor.transform.localPosition = new Vector3(0, -Constants.HexagonBlockDefaultHeight, -1);

            _towerWindow1 = Instantiate(towerPartsData.towerWindow, transform);
            _towerWindow1.transform.localPosition = new Vector3(0.8f, 1, -0.5f);
            _towerWindow1.transform.localRotation = Quaternion.Euler(0, -60, 0);

            _towerWindow2 = Instantiate(towerPartsData.towerWindow, transform);
            _towerWindow2.transform.localPosition = new Vector3(-0.8f, 1, -0.5f);
            _towerWindow2.transform.localRotation = Quaternion.Euler(0, 60, 0);

            SetTowerComponentsActive(false);
        }

        private void SetTowerComponentsActive(bool active)
        {
            if (_towerTopHole) _towerTopHole.gameObject.SetActive(active);
            if (_towerDoor) _towerDoor.SetActive(active);
            if (_towerWindow1) _towerWindow1.SetActive(active);
            if (_towerWindow2) _towerWindow2.SetActive(active);
        }

        private void ConvertToDefaultBlock()
        {
            if (_hexagonStack.Count <= 0) _towerTopHole.gameObject.SetActive(false);
            if (!_towerDoor.activeSelf || !_towerWindow1.activeSelf || !_towerWindow2.activeSelf) return;

            _towerDoor.SetActive(false);
            _towerWindow1.SetActive(false);
            _towerWindow2.SetActive(false);
        }

        private void ConvertToTower() => SetTowerComponentsActive(true);

        private void UpdateTowerComponents()
        {
            float topElementY = _hexagonStack.Count * (Constants.HexagonElementColliderSize.y + Constants.HexagonElementSpacing);
            _towerTopHole.transform.localPosition = new Vector3(0, topElementY, 0);

            if (_towerTopHole.gameObject.activeSelf && TopElement) _towerTopHole.OnTowerUpdate(_hexagonStack.Count, TopElement.HexagonElementData.HexagonType);
        }

        private void UpdateColliderSize()
        {
            if (!_boxCollider) return;

            var newHeight = _hexagonStack.Count * (Constants.HexagonElementColliderSize.y + Constants.HexagonElementSpacing);
            _boxCollider.size = new Vector3(Constants.HexagonElementColliderSize.x, newHeight, Constants.HexagonElementColliderSize.z);

            var newCenterY = (newHeight - Constants.HexagonElementColliderSize.y) / 2f;
            _boxCollider.center = new Vector3(0, newCenterY, 0);
        }

        private void SetColliderEnabled(bool canEnable)
        {
            _boxCollider.enabled = canEnable;
        }

        private void UnsubscribeActions()
        {
            Extensions.UnsubscribeAction(BlockOnNullInBattle);
        }

        #endregion

        #region Animation

        private async void MoveElementWithAnimation(HexagonElement element, Vector3 targetPosition)
        {
            _elementMoveTween = element.transform.DOLocalJump(targetPosition, .5f, 1, 0.25f);
            await Task.Delay(250);
            _elementMoveTween.OnComplete(UpdateTowerComponents);
        }

        #endregion
    }
}