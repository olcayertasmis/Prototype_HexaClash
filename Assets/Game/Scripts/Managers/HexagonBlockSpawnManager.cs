using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Data.SOData;
using HexaClash.Game.Scripts.Data.ValueData;
using HexaClash.Game.Scripts.Gameplay.HexagonSystem;
using HexaClash.Game.Scripts.Interfaces;
using UnityEngine;

namespace HexaClash.Game.Scripts.Managers
{
    public class HexagonBlockSpawnManager : ManagerBase
    {
        #region Serialized Fields

        [Header("Spawn Points")]
        [SerializeField] private HexagonBlockSpawnSlotData[] hexagonBlockSpawnSlots;

        [Header("Data")]
        [SerializeField] private TowerPartsData towerPartsData;

        #endregion

        #region Private Fields

        private Queue<HexagonBlockData> _blockQueue;

        #endregion

        #region Helpers

        private void SetHexagonBlockSlotStatus(HexagonBlockSpawnSlotData spawnSlotData, bool status) => spawnSlotData.isAvailable = status;

        private bool AreAllSlotsAvailable() => hexagonBlockSpawnSlots.All(hexagonBlockSpawnSlot => hexagonBlockSpawnSlot.isAvailable);

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

        #region Private Methods

        private void SubscribeLevelManager(IMonoBehaviourSystem levelManager)
        {
            var manager = levelManager as LevelManager;
            if (manager) manager.OnLevelStarted += OnLevelStarted;
        }

        private async void OnLevelStarted(LevelSettingsSO levelSettings, int levelIndex)
        {
            await SetAllHexagonBlockStatus(true);
            InitializeBlockQueue(levelSettings.HexagonBlockLevel);
            SpawnInitialBlocks();
        }

        private Task SetAllHexagonBlockStatus(bool status)
        {
            foreach (var hexagonBlockSpawnSlot in hexagonBlockSpawnSlots)
            {
                SetHexagonBlockSlotStatus(hexagonBlockSpawnSlot, status);
            }

            return Task.CompletedTask;
        }

        private void InitializeBlockQueue(HexagonBlockLevelData hexagonBlockLevel)
        {
            _blockQueue = new Queue<HexagonBlockData>();
            foreach (var blockData in hexagonBlockLevel.GetHexagonBlockData) _blockQueue.Enqueue(blockData);
        }

        private void SpawnInitialBlocks()
        {
            if (_blockQueue.Count <= 0) return;

            foreach (var hexagonBlockSpawnSlot in hexagonBlockSpawnSlots)
            {
                if (hexagonBlockSpawnSlot.isAvailable) SpawnHexagonBlock(hexagonBlockSpawnSlot);
            }
        }

        private void SpawnHexagonBlock(HexagonBlockSpawnSlotData hexagonBlockSpawnSlotData)
        {
            if (_blockQueue.Count == 0) return;

            HexagonBlockData blockData = _blockQueue.Dequeue();
            HexagonBlockFactory.CreateHexagonBlock(hexagonBlockSpawnSlotData, blockData, towerPartsData);
            SetHexagonBlockSlotStatus(hexagonBlockSpawnSlotData, false);
        }

        #endregion

        #region Public Methods

        public void OnBlockPlaced(HexagonBlock hexagonBlock)
        {
            SetHexagonBlockSlotStatus(hexagonBlock.HexagonBlockSpawnSlot, true);

            if (_blockQueue.Count > 0 && AreAllSlotsAvailable()) SpawnInitialBlocks();
        }

        #endregion
    }
}