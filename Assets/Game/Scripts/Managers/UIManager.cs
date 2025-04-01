using System.Threading.Tasks;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Data.SOData;
using HexaClash.Game.Scripts.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexaClash.Game.Scripts.Managers
{
    public class UIManager : ManagerBase
    {
        #region Serialized Fields

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private Button battleButton;

        [Header("Panels")]
        [SerializeField] private GameObject losePanel;
        [SerializeField] private GameObject winPanel;

        #endregion

        #region Override Methods

        public override void InitializeSystem()
        {
            InitializeSystemBody();
        }

        protected override void InitializeSystemBody()
        {
            base.InitializeSystemBody();

            var levelManager = GameManager.Instance.GetSystem<LevelManager>();
            if (levelManager.IsInitialized) levelManager.OnLevelStarted += OnLevelStarted;
            else levelManager.OnInitializeComplete += SubscribeLevelManager;

            var inputManager = GameManager.Instance.GetSystem<InputManager>();
            if (inputManager.IsInitialized)
            {
                inputManager.OnMovesUpdated += UpdateMovesText;
                inputManager.OnBattleButtonActivated += ActivateBattleButton;
            }
            else inputManager.OnInitializeComplete += SubscribeInputManager;

            battleButton.onClick.AddListener(OnBattleButtonClicked);
            battleButton.gameObject.SetActive(false);
        }

        public override void ReleaseSystem()
        {
            base.ReleaseSystem();

            var manager = GameManager.Instance.GetSystem<LevelManager>();
            if (manager) manager.OnLevelStarted -= OnLevelStarted;

            var inputManager = GameManager.Instance.GetSystem<InputManager>();
            if (inputManager)
            {
                inputManager.OnMovesUpdated -= UpdateMovesText;
                inputManager.OnBattleButtonActivated -= ActivateBattleButton;
            }

            battleButton.onClick.RemoveAllListeners();
        }

        public async void GameEndPanelActive(bool isWin)
        {
            await Task.Delay(1000);

            if (isWin) winPanel.SetActive(true);
            else losePanel.SetActive(true);
        }

        #endregion

        #region Private Methods

        private void OnLevelStarted(LevelSettingsSO levelSettings, int levelIndex)
        {
            UpdateMovesText(levelSettings.HexagonBlockLevel.MoveCount);
            UpdateLevelText(levelIndex);
        }

        private void SubscribeLevelManager(IMonoBehaviourSystem levelManager)
        {
            var manager = levelManager as LevelManager;
            if (manager) manager.OnLevelStarted += OnLevelStarted;
        }

        private void SubscribeInputManager(IMonoBehaviourSystem inputManager)
        {
            var manager = inputManager as InputManager;
            if (manager)
            {
                manager.OnMovesUpdated += UpdateMovesText;
                manager.OnBattleButtonActivated += ActivateBattleButton;
            }
        }

        private void UpdateLevelText(int levelIndex)
        {
            levelText.text = $"Level: {levelIndex + 1}";
        }

        private void UpdateMovesText(int moves)
        {
            movesText.text = $"Moves: {moves}";
        }

        private void ActivateBattleButton()
        {
            battleButton.gameObject.SetActive(true);
        }

        private void OnBattleButtonClicked()
        {
            var battleManager = GameManager.Instance.GetSystem<BattleManager>();
            battleManager?.StartBattle();

            battleButton.gameObject.SetActive(false);
        }

        #endregion
    }
}