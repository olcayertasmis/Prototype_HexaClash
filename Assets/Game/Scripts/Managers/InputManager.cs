using System;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Data.SOData;
using HexaClash.Game.Scripts.Gameplay.HexagonSystem;
using HexaClash.Game.Scripts.Interfaces;
using UnityEngine;

namespace HexaClash.Game.Scripts.Managers
{
    public class InputManager : ManagerBase
    {
        #region Serialized Fields

        [Header("Camera")]
        [SerializeField] private Camera cameraMain;

        #endregion

        #region Private Fields

        [Header("Data")]
        private HexagonBlock _selectedHexagonBlock;
        private int _currentMovesCount;

        [Header("Controls")]
        private bool _isDragging;

        #endregion

        #region Events

        public Action<int> OnEndDrag;
        public Action<int> OnMovesUpdated;
        public Action OnBattleButtonActivated;

        #endregion

        #region Override Methods

        public override void InitializeSystem()
        {
            InitializeSystemBody();
        }

        protected override void InitializeSystemBody()
        {
            base.InitializeSystemBody();

            var manager = GameManager.Instance.GetSystem<LevelManager>();
            if (manager.IsInitialized) manager.OnLevelStarted += OnLevelStarted;
            else manager.OnInitializeComplete += SubscribeLevelManager;
        }

        public override void ReleaseSystem()
        {
            base.ReleaseSystem();

            var manager = GameManager.Instance.GetSystem<LevelManager>();
            if (manager) manager.OnLevelStarted -= OnLevelStarted;
        }

        #endregion

        #region Unity Methods

        private void Update()
        {
            HandleTouchInput();
        }

        #endregion

        #region Private Methods

        private void OnLevelStarted(LevelSettingsSO levelSettings, int levelIndex)
        {
            _currentMovesCount = levelSettings.HexagonBlockLevel.MoveCount;
        }

        private void SubscribeLevelManager(IMonoBehaviourSystem levelManager)
        {
            var manager = levelManager as LevelManager;
            if (manager) manager.OnLevelStarted += OnLevelStarted;
        }

        private void HandleTouchInput()
        {
            if (_currentMovesCount <= 0) return;

            if (Input.GetMouseButtonDown(0)) StartDrag();

            if (_isDragging) Drag();

            if (Input.GetMouseButtonUp(0)) EndDrag();
        }

        private void StartDrag()
        {
            var ray = cameraMain.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider.TryGetComponent(out HexagonBlock hexagonBlock))
                {
                    if (hexagonBlock.IsPlaced) return;

                    _selectedHexagonBlock = hexagonBlock;
                    hexagonBlock.OnDragStart();

                    _isDragging = true;
                    Debug.Log($"Started dragging: {hexagonBlock.name}");
                }
            }
        }

        private void Drag()
        {
            if (!_selectedHexagonBlock) return;

            var ray = cameraMain.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                Vector3 newPosition = hit.point;
                newPosition.y = Constants.HexagonBlockDefaultHeight;
                _selectedHexagonBlock.transform.position = newPosition;
            }
        }

        private void EndDrag()
        {
            if (!_selectedHexagonBlock) return;

            var ray = cameraMain.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider.TryGetComponent(out HexagonArea hexagonArea) && !hexagonArea.HexagonBlock && !hexagonArea.IsLocked)
                {
                    hexagonArea.PlaceHexagonBlock(_selectedHexagonBlock);
                    _currentMovesCount--;

                    OnMovesUpdated?.Invoke(_currentMovesCount);

                    if (_currentMovesCount <= 0) OnBattleButtonActivated?.Invoke();
                }
                else _selectedHexagonBlock.ResetPosition();
            }

            _selectedHexagonBlock.OnDragEnd();
            _selectedHexagonBlock = null;
            _isDragging = false;
            Debug.Log("Ended dragging.");
        }

        #endregion
    }
}