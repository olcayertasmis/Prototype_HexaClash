using System;

namespace HexaClash.Game.Scripts.Interfaces
{
    public interface IBattleController
    {
        event Action OnBattleStart;
        event Action OnBattleEnd;
        event Action<bool> OnBattleResult;

        bool IsBattleActive { get; }

        void StartBattle();
        void StopBattle();
    }
}