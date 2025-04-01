using System;
using System.Collections.Generic;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Interfaces;
using UnityEngine;

namespace HexaClash.Game.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton

        public static GameManager Instance;

        #endregion

        [Header("Managers")]
        [SerializeField] private ManagerBase[] managers;
        private List<IMonoBehaviourSystem> _managers;

        #region Unity Methods

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;

            AddSystems();
            InitializeSystems();
        }

        private void OnDestroy()
        {
            foreach (var item in _managers) item.ReleaseSystem();
        }

        #endregion

        private void AddSystems()
        {
            _managers = new List<IMonoBehaviourSystem>();

            foreach (var managerBase in managers)
            {
                if (managerBase.TryGetComponent(out IMonoBehaviourSystem system)) _managers.Add(system);
            }
        }

        private void InitializeSystems()
        {
            foreach (var system in _managers)
            {
                try
                {
                    system.InitializeSystem();
                }
                catch (Exception e)
                {
                    system.OnInitializeFailed(e);
                    Debug.LogError($"An exception happened while Initializing {system.SystemName}. Exception : \n {e}");
                }
            }
        }

        public T GetSystem<T>() where T : IMonoBehaviourSystem
        {
            foreach (var system in _managers)
            {
                if (system is T monoBehaviourSystem) return monoBehaviourSystem;
            }

            return default;
        }
    }
}