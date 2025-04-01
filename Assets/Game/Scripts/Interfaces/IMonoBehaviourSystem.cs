using System;

namespace HexaClash.Game.Scripts.Interfaces
{
    public interface IMonoBehaviourSystem
    {
        public bool IsInitialized { get; }

        public string SystemName { get; }

        public Action<IMonoBehaviourSystem> OnInitializeComplete { get; set; }

        public void InitializeSystem();

        public void OnInitializeFailed(Exception e);

        public void ReleaseSystem();
    }
}