using System;
using HexaClash.Game.Scripts.Interfaces;
using UnityEngine;

namespace HexaClash.Game.Scripts.Core
{
    public abstract class ManagerBase : MonoBehaviour, IMonoBehaviourSystem
    {
        public bool IsInitialized { get; private set; }
        public string SystemName => GetType().ToString();

        public Action<IMonoBehaviourSystem> OnInitializeComplete { get; set; }

        public abstract void InitializeSystem();

        protected virtual void InitializeSystemBody()
        {
            if (IsInitialized) return;

            IsInitialized = true;
            OnInitializeComplete?.Invoke(this);
            OnInitializeComplete = null;
        }

        public virtual void OnInitializeFailed(Exception e)
        {
            Debug.LogError($"Initialization failed for {SystemName}: {e.Message}");
            return;
        }

        public virtual void ReleaseSystem()
        {
            return;
        }
    }
}