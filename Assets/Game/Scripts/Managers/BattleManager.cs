using System;
using System.Collections;
using System.Collections.Generic;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Gameplay.Battle;
using HexaClash.Game.Scripts.Gameplay.CharacterSystem;
using HexaClash.Game.Scripts.Gameplay.HexagonSystem;
using HexaClash.Game.Scripts.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexaClash.Game.Scripts.Managers
{
    public class BattleManager : ManagerBase, IBattleController
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private CharacterSpawner characterSpawner;

        #endregion

        #region Private Fields

        private readonly List<Character> _allyCharacters = new();
        private readonly List<Character> _enemyCharacters = new();
        private readonly List<HexagonBlock> _hexagonBlocks = new();
        private Coroutine _battleRoutine;
        private Coroutine _allySpawnRoutine;
        private Coroutine _enemySpawnRoutine;

        #endregion

        #region Events

        public event Action OnBattleStart;
        public event Action OnBattleEnd;
        public event Action<bool> OnBattleResult; // true for player win, false for enemy win

        #endregion

        #region Helpers

        public bool IsBattleActive => _battleRoutine != null;

        #endregion

        #region Override Methods

        public override void InitializeSystem()
        {
            InitializeSystemBody();

            ClearBattleState();

            if (characterSpawner) characterSpawner.OnCharacterSpawned += HandleCharacterSpawned;
        }

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            if (characterSpawner) characterSpawner.OnCharacterSpawned -= HandleCharacterSpawned;
        }

        #endregion

        #region Public Methods

        public void StartBattle()
        {
            if (IsBattleActive) return;

            ClearBattleState();
            FindAllHexagonBlocks();

            var dataManager = GameManager.Instance.GetSystem<CharacterDataManager>();
            var levelManager = GameManager.Instance.GetSystem<LevelManager>();

            if (characterSpawner && dataManager && levelManager)
            {
                //StartCoroutine(StartBattleSequence(dataManager, levelManager));

                StartCoroutine(characterSpawner.SpawnAllies(_hexagonBlocks, dataManager));
                StartCoroutine(characterSpawner.SpawnEnemies(levelManager.GetCurrentLevelSettings(), dataManager));

                _battleRoutine = StartCoroutine(BattleRoutine());
            }
        }

        public void StopBattle()
        {
            if (!IsBattleActive) return;

            SafeStopCoroutine(ref _allySpawnRoutine);
            SafeStopCoroutine(ref _enemySpawnRoutine);
            SafeStopCoroutine(ref _battleRoutine);

            ClearBattleState();
        }

        #endregion

        #region Private Methods

        private void HandleCharacterSpawned(Character character)
        {
            if (character is AllyCharacter)
            {
                _allyCharacters.Add(character);
                //character.IsInitialized = true;
            }
            else if (character is Enemy)
            {
                _enemyCharacters.Add(character);
            }
        }

        private void ClearBattleState()
        {
            _hexagonBlocks.Clear();
            _allyCharacters.Clear();
            _enemyCharacters.Clear();
        }

        private void SafeStopCoroutine(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private void FindAllHexagonBlocks()
        {
            _hexagonBlocks.Clear();
            _hexagonBlocks.AddRange(FindObjectsOfType<HexagonBlock>());
        }

        private IEnumerator BattleRoutine()
        {
            OnBattleStart?.Invoke();

            yield return new WaitForSeconds(.5f);

            while (_allyCharacters.Count > 0 && _enemyCharacters.Count > 0)
            {
                UpdateCharacterTargets();
                CleanupDeadCharacters();

                yield return new WaitForSeconds(Constants.BattleTickRate);
            }

            HandleBattleEnd();
        }

        private void UpdateCharacterTargets()
        {
            foreach (var allyCharacter in _allyCharacters)
            {
                if (_enemyCharacters.Count > 0)
                {
                    Character targetEnemy = GetRandomTarget(_enemyCharacters);
                    allyCharacter.SetTarget(targetEnemy);
                }
            }

            foreach (var enemyCharacter in _enemyCharacters)
            {
                if (_allyCharacters.Count > 0)
                {
                    Character targetAlly = GetRandomTarget(_allyCharacters);
                    enemyCharacter.SetTarget(targetAlly);
                }
            }
        }

        private Character GetRandomTarget(IReadOnlyList<Character> characters)
        {
            return characters.Count > 0 ? characters[Random.Range(0, characters.Count)] : null;
        }

        private void CleanupDeadCharacters()
        {
            _allyCharacters.RemoveAll(character => character.IsDead);
            _enemyCharacters.RemoveAll(character => character.IsDead);
        }

        private void HandleBattleEnd()
        {
            bool playerWon = _allyCharacters.Count > 0;

            OnBattleEnd?.Invoke();
            OnBattleResult?.Invoke(playerWon);

            var uiManager = GameManager.Instance.GetSystem<UIManager>();
            if (uiManager)
            {
                uiManager.GameEndPanelActive(playerWon);
            }

            Debug.Log(playerWon ? "Player Wins!" : "Enemy Wins!");
        }

        #endregion
    }
}