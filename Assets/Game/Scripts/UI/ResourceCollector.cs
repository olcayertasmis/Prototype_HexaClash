using DG.Tweening;
using HexaClash.Game.Scripts.Data.ValueData;
using HexaClash.Game.Scripts.Interfaces;
using HexaClash.Game.Scripts.Managers;
using System;
using System.Threading.Tasks;
using HexaClash.Game.Scripts.Data.EnumData;
using UnityEngine;
using UnityEngine.UI;

namespace HexaClash.Game.Scripts.UI
{
    public class ResourceCollector : MonoBehaviour, IResourceCollector
    {
        [Header("References")]
        [SerializeField] private Image tempImagePrefab;
        [SerializeField] private Transform animationContainer;

        [Header("Animation Settings")]
        [SerializeField] private float spawnDelay = 0.05f;
        [SerializeField] private float initialMoveDuration = 0.5f;
        [SerializeField] private float finalMoveDuration = 0.75f;
        [SerializeField] private float randomSpread = 250f;

        #region Public Methods

        public async Task AnimateResourceCollection(ResourceAnimationData animationData)
        {
            int resourceImageCount = CalculateOptimalImageCount(animationData.Count);

            for (int i = 0; i < resourceImageCount; i++)
            {
                CreateAndAnimateResource(animationData);
                await Task.Delay(TimeSpan.FromSeconds(spawnDelay));
            }
        }

        public Vector3 GetTargetPosition(ResourceType type) => GameManager.Instance.GetSystem<ResourceUIManager>().GetResourceScreenPosition(type);

        #endregion

        #region Private Methods

        private void CreateAndAnimateResource(ResourceAnimationData data)
        {
            var resourceImage = Instantiate(tempImagePrefab, animationContainer);
            resourceImage.name = "ResourceAnimation";

            resourceImage.sprite = data.Sprite;
            resourceImage.SetNativeSize();

            resourceImage.rectTransform.anchoredPosition = data.StartPosition;
            resourceImage.transform.localScale = Vector3.one * data.StartScale;

            Sequence animationSequence = DOTween.Sequence();

            animationSequence.Append(resourceImage.transform.DOScale(data.MidScale, initialMoveDuration));

            animationSequence.Join(resourceImage.rectTransform.DOAnchorPos(data.StartPosition + GetRandomOffset(), initialMoveDuration).SetEase(Ease.OutSine));

            animationSequence.Append(resourceImage.transform.DOScale(data.TargetScale, finalMoveDuration));

            animationSequence.Join(resourceImage.rectTransform.DOAnchorPos(data.TargetPosition, finalMoveDuration).SetEase(Ease.InSine));

            animationSequence.OnComplete(() =>
            {
                data.OnComplete?.Invoke();
                Destroy(resourceImage.gameObject);
            });
        }

        private Vector2 GetRandomOffset() => new(UnityEngine.Random.Range(-randomSpread, randomSpread), UnityEngine.Random.Range(-randomSpread, randomSpread));

        private int CalculateOptimalImageCount(int totalAmount)
        {
            const int minVisualCount = 1;
            const int maxVisualCount = 10;
            const int amountPerVisual = 10;

            return Mathf.Clamp(Mathf.CeilToInt(totalAmount / (float)amountPerVisual), minVisualCount, maxVisualCount);
        }

        #endregion
    }
}