using System;
using System.Collections.Generic;
using HexaClash.Game.Scripts.Data.EnumData;
using UnityEngine;

namespace HexaClash.Game.Scripts.Managers
{
    public class ResourceManager
    {
        #region Variables

        public Action<ResourceType, int> OnResourceUpdated;

        private readonly Dictionary<ResourceType, int> _resources = new();

        #endregion

        #region Public Methods

        public void UpdateResource(ResourceType resourceType, int amount)
        {
            _resources.TryAdd(resourceType, 0);

            _resources[resourceType] = Mathf.Max(0, _resources[resourceType] + amount);
            OnResourceUpdated?.Invoke(resourceType, _resources[resourceType]);
        }

        public bool HasEnoughResource(ResourceType resourceType, int amount)
        {
            return _resources.ContainsKey(resourceType) && _resources[resourceType] >= amount;
        }

        public void SaveResources()
        {
            foreach (var resource in _resources)
            {
                PlayerPrefs.SetInt(resource.Key.ToString(), resource.Value);
            }

            PlayerPrefs.Save();
        }

        public void LoadResources()
        {
            _resources.Clear();

            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                string key = resourceType.ToString();
                if (PlayerPrefs.HasKey(resourceType.ToString()))
                {
                    _resources[resourceType] = PlayerPrefs.GetInt(key);
                    OnResourceUpdated?.Invoke(resourceType, _resources[resourceType]);
                }
                else
                {
                    _resources[resourceType] = 0;
                    OnResourceUpdated?.Invoke(resourceType, _resources[resourceType]);
                }
            }
        }

        #endregion
        
    }
}