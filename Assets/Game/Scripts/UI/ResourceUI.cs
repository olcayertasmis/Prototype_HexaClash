using HexaClash.Game.Scripts.Data.EnumData;
using TMPro;
using UnityEngine;

namespace HexaClash.Game.Scripts.UI
{
    public class ResourceUI : MonoBehaviour
    {
        #region Variables

        [Header("References")]
        [SerializeField] private TextMeshProUGUI resourceText;

        [Header("Data")]
        [SerializeField] private ResourceType resourceType;
        [SerializeField] private RectTransform iconTransform;

        #endregion

        #region Helpers

        public ResourceType GetResourceType() => resourceType;

        #endregion

        #region Public Methods

        public Vector2 GetIconScreenPosition()
        {
            return RectTransformUtility.WorldToScreenPoint(GetComponentInParent<Canvas>().worldCamera, iconTransform.position);
        }

        public void HandleResourceUpdated(ResourceType updatedResourceType, int amount)
        {
            if (updatedResourceType == resourceType) UpdateResourceUI(amount);
        }

        #endregion

        #region Private Methods

        private void UpdateResourceUI(int amount) => resourceText.text = amount.ToString();

        #endregion
    }
}