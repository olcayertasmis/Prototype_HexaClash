using HexaClash.Game.Scripts.Data.SOData.CharacterDatasSO;

namespace HexaClash.Game.Scripts.Gameplay.CharacterSystem
{
    public class AllyCharacter : Character
    {
        public override void Initialize(CharacterDataSO characterData)
        {
            base.Initialize(characterData);

            MoveSpeed = characterData?.MoveSpeed ?? 0f;
        }
    }
}