using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Data.SOData;
using HexaClash.Game.Scripts.Data.SOData.CharacterDatasSO;
using HexaClash.Game.Scripts.Gameplay.CharacterSystem;
using HexaClash.Game.Scripts.Gameplay.HexagonSystem;
using HexaClash.Game.Scripts.Interfaces;
using HexaClash.Game.Scripts.Managers;
using HexaClash.Game.Scripts.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexaClash.Game.Scripts.Gameplay.Battle
{
    public class CharacterSpawner : MonoBehaviour, ICharacterSpawner
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private ResourceCollector resourceCollector;

        [Header("Settings")]
        [SerializeField] private float allySpawnAnimationDuration;
        [SerializeField] private float enemySpawnDelay;

        [Header("Prefabs")]
        [SerializeField] private GameObject allyCharacterPrefab;
        [SerializeField] private GameObject enemyPrefab;

        #endregion

        #region Actions

        public event Action<Character> OnCharacterSpawned;

        #endregion

        public IEnumerator SpawnAllies(IEnumerable<HexagonBlock> hexagonBlocks, CharacterDataManager dataManager)
        {
            if (!dataManager) yield break;

            foreach (var block in hexagonBlocks)
            {
                int elementCount = block.GetElements().Count;

                for (int i = 0; i < elementCount; i++)
                {
                    HexagonElement topElement = block.TopElement;
                    yield return block.RemoveTopElementCoroutine((removedElement) => { topElement = removedElement; }, true);

                    if (!topElement) continue;

                    CharacterDataSO data = dataManager.GetAllyCharacterData(topElement.AllyCharacterType);
                    if (!data) continue;

                    var allyCharacter = SpawnCharacter(allyCharacterPrefab, topElement.transform.position, data);
                    OnCharacterSpawned?.Invoke(allyCharacter);

                    yield return AnimateCharacterSpawn(allyCharacter.transform);

                    allyCharacter.IsInitialized = true;
                }
            }
        }

        public IEnumerator SpawnEnemies(LevelSettingsSO levelSettings, CharacterDataManager dataManager)
        {
            if (!levelSettings || !dataManager) yield break;

            foreach (var enemySpawnSetting in levelSettings.EnemySpawnSettings)
            {
                for (int i = 0; i < enemySpawnSetting.SpawnCount; i++)
                {
                    Vector3 spawnPosition = new Vector3(Random.Range(Constants.EnemySpawnXRangeMin, Constants.EnemySpawnXRangeMax), Constants.CharacterFinalYPosition, Constants.EnemySpawnZPosition);

                    EnemyDataSO data = dataManager.GetEnemyData(enemySpawnSetting.EnemyType);
                    if (!data) continue;

                    var enemy = SpawnCharacter(enemyPrefab, spawnPosition, data);
                    enemy.GetComponent<Enemy>().EnemyInitialize(resourceCollector);
                    OnCharacterSpawned?.Invoke(enemy);

                    yield return new WaitForSeconds(enemySpawnDelay);
                }
            }
        }

        public Character SpawnCharacter(GameObject prefab, Vector3 position, CharacterDataSO data)
        {
            if (!prefab || !data) return null;

            GameObject characterObject = Instantiate(prefab, position, Quaternion.Euler(60, 0, 0));

            if (!characterObject) return null;

            Character character = characterObject.GetComponent<Character>();
            if (!character) return null;

            character.Initialize(data);
            return character;
        }

        public IEnumerator AnimateCharacterSpawn(Transform characterTransform)
        {
            Vector3 originalPosition = characterTransform.position;
            Vector3 intermediatePosition = new Vector3(originalPosition.x, originalPosition.y + Constants.CharacterSpawnYOffset, originalPosition.z + Constants.CharacterSpawnZOffset);

            Vector3 finalPosition = new Vector3(originalPosition.x, Constants.CharacterFinalYPosition, originalPosition.z + Constants.CharacterSpawnZOffset);

            characterTransform.DOMove(intermediatePosition, allySpawnAnimationDuration);
            yield return new WaitForSeconds(allySpawnAnimationDuration);

            characterTransform.DOMove(finalPosition, allySpawnAnimationDuration);
            yield return new WaitForSeconds(allySpawnAnimationDuration);
        }
    }
}