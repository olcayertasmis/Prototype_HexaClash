using System;
using System.Collections;
using System.Collections.Generic;
using HexaClash.Game.Scripts.Data.SOData;
using HexaClash.Game.Scripts.Data.SOData.CharacterDatasSO;
using HexaClash.Game.Scripts.Gameplay.CharacterSystem;
using HexaClash.Game.Scripts.Gameplay.HexagonSystem;
using HexaClash.Game.Scripts.Managers;
using UnityEngine;

namespace HexaClash.Game.Scripts.Interfaces
{
    public interface ICharacterSpawner
    {
        event Action<Character> OnCharacterSpawned;

        IEnumerator SpawnAllies(IEnumerable<HexagonBlock> hexagonBlocks, CharacterDataManager dataManager);
        IEnumerator SpawnEnemies(LevelSettingsSO levelSettings, CharacterDataManager dataManager);
        Character SpawnCharacter(GameObject prefab, Vector3 position, CharacterDataSO data);
        IEnumerator AnimateCharacterSpawn(Transform characterTransform);
    }
}