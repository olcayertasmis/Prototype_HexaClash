using System.Threading.Tasks;
using HexaClash.Game.Scripts.Data.EnumData;
using HexaClash.Game.Scripts.Data.SOData.CharacterDatasSO;
using HexaClash.Game.Scripts.Data.ValueData;
using HexaClash.Game.Scripts.Interfaces;
using HexaClash.Game.Scripts.Managers;
using HexaClash.Game.Scripts.UI;
using UnityEngine;

namespace HexaClash.Game.Scripts.Gameplay.CharacterSystem
{
    public class Enemy : Character, IResourceDropper
    {
        [SerializeField] private ResourceData resourceData;
        private ResourceCollector _resourceCollector;

        #region Override Methods

        public override void Initialize(CharacterDataSO characterData)
        {
            base.Initialize(characterData);

            var enemyData = characterData as EnemyDataSO;
            MoveSpeed = enemyData?.onStartMoveSpeed ?? characterData.MoveSpeed;

            Invoke(nameof(ChangeMoveSpeedToNormal), 1f);
            IsInitialized = true;
        }

        protected override async void Die()
        {
            await DropResources();

            base.Die();
        }

        #endregion

        #region Public Methods

        public void EnemyInitialize(ResourceCollector resourceCollector) => SetResourceCollector(resourceCollector);

        #endregion

        #region Private Methods

        private async Task DropResources()
        {
            var resourceUIManager = GameManager.Instance.GetSystem<ResourceUIManager>();
            var resourceManager = resourceUIManager.GetResourceManager();
            var canvas = resourceUIManager.GetParentCanvas();

            var screenPos = Camera.main.WorldToScreenPoint(GetDropPosition());

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPos, canvas.worldCamera, out var uiPosition);

            var animData = new ResourceAnimationData(resourceData.icon, uiPosition, _resourceCollector.GetTargetPosition(resourceData.type), resourceData.amount, 1.5f, 2f, 1.2f,
                () => resourceManager.UpdateResource(ResourceType.Gold, resourceData.amount));

            await _resourceCollector.AnimateResourceCollection(animData);
        }

        private void SetResourceCollector(ResourceCollector collector) => _resourceCollector = collector;

        private void ChangeMoveSpeedToNormal() => MoveSpeed = CharacterData.MoveSpeed;

        #endregion

        #region IResourceDropper Implementation

        public ResourceData GetResourceData() => resourceData;
        public Vector3 GetDropPosition() => transform.position + Vector3.up * 1.5f;

        #endregion
    }
}