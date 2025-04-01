using System.Collections.Generic;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Data.EnumData;
using HexaClash.Game.Scripts.UI;
using UnityEngine;

namespace HexaClash.Game.Scripts.Managers
{
    public class ResourceUIManager : ManagerBase
    {
        #region Variables

        [Header("Resources UI")]
        [SerializeField] private List<ResourceUI> resourceUIList = new();

        [Header("References")]
        [SerializeField] private Canvas parentCanvas;
        private ResourceManager _resourceManager = new();

        [Header("Data")]
        private readonly Dictionary<ResourceType, ResourceUI> _resourceUIMap = new();

        #endregion

        #region Helpers

        public ref ResourceManager GetResourceManager() => ref _resourceManager;
        public Canvas GetParentCanvas() => parentCanvas;

        #endregion

        #region Override Methods

        public override void InitializeSystem()
        {
            InitializeResourceUI();

            InitializeSystemBody();
        }

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            _resourceManager.SaveResources();

            foreach (var resourceUI in resourceUIList)
            {
                _resourceManager.OnResourceUpdated -= resourceUI.HandleResourceUpdated;
            }
        }

        #endregion

        #region Public Methods

        public Vector2 GetResourceScreenPosition(ResourceType type)
        {
            if (_resourceUIMap.TryGetValue(type, out var resourceUI))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.GetComponent<RectTransform>(), resourceUI.GetIconScreenPosition(), parentCanvas.worldCamera, out var localPoint);

                return localPoint;
            }

            return Vector2.zero;
        }

        #endregion

        #region Private Methods

        private void InitializeResourceUI()
        {
            _resourceManager.LoadResources();

            foreach (var resourceUI in resourceUIList)
            {
                _resourceManager.OnResourceUpdated += resourceUI.HandleResourceUpdated;
            }

            foreach (var resourceUI in resourceUIList)
            {
                _resourceUIMap[resourceUI.GetResourceType()] = resourceUI;
                _resourceManager.UpdateResource(resourceUI.GetResourceType(), 0);
            }
        }

        #endregion
    }
}