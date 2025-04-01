using UnityEngine;

namespace HexaClash.Game.Scripts.Data.SOData.CharacterDatasSO
{
    public class CharacterDataSO : ScriptableObject
    {
        #region Data

        [Header("Data")]
        [SerializeField] private int health;
        [SerializeField] private int attackPower;
        [SerializeField] private float attackRange;
        [SerializeField] private float moveSpeed;

        [Header("References")]
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private AnimatorOverrideController animatorOverrideController;

        #endregion

        #region Public Properties

        public int Health => health;
        public int AttackPower => attackPower;
        public float AttackRange => attackRange;
        public float MoveSpeed => moveSpeed;
        public Sprite CharacterSprite => characterSprite;
        public AnimatorOverrideController AnimatorOverrideController => animatorOverrideController;

        #endregion
    }
}