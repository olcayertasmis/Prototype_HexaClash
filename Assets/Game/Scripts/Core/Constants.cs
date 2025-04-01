using UnityEngine;

namespace HexaClash.Game.Scripts.Core
{
    public static class Constants
    {
        #region Hexagon Element

        public static readonly Vector3 HexagonElementColliderSize = new(2.002692f, 0.1691939f, 1.732344f);
        public const float HexagonElementSpacing = 0.1f;

        #endregion

        #region Input Manager

        public const float HexagonBlockDefaultHeight = 0.15f;

        #endregion

        #region Hexagon Block

        public const int MaxHexagonElementsPerBlock = 20;
        public const int ElementsRequiredForMediumTower = 15;
        public const int ElementsRequiredForTower = 10;

        #endregion

        #region Hexagon Area

        public const int HexagonAreaUnlockRequirement = 6;

        #endregion

        #region Battle && Character Spawn System

        public const float BattleTickRate = 0.1f;

        public const float CharacterSpawnYOffset = 0.5f;
        public const float CharacterSpawnZOffset = 4f;
        public const float CharacterFinalYPosition = 0.61f;
        public const float EnemySpawnXRangeMin = -7f;
        public const float EnemySpawnXRangeMax = 7.5f;
        public const float EnemySpawnZPosition = 36f;

        #endregion
    }
}