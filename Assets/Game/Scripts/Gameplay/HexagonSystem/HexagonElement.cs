using HexaClash.Game.Scripts.Data.EnumData;
using HexaClash.Game.Scripts.Data.SOData;
using UnityEngine;
using UnityEngine.UI;

namespace HexaClash.Game.Scripts.Gameplay.HexagonSystem
{
    public class HexagonElement : MonoBehaviour
    {
        #region SerializeField Fields

        [Header("References")]
        [SerializeField] private Image characterImage;

        #endregion

        #region Properties

        public AllyCharacterType AllyCharacterType { get; private set; }
        public HexagonElementDataSO HexagonElementData { get; private set; }

        #endregion

        #region Public Methods

        public void Initialize(HexagonElementDataSO hexagonElementDataSO)
        {
            if (!hexagonElementDataSO) return;

            HexagonElementData = hexagonElementDataSO;
            AllyCharacterType = hexagonElementDataSO.GetAllyCharacterDataSO.GetAllyCharacterType;

            SetCanvasActive(false);

            SetHexagonCharacterImage(hexagonElementDataSO.GetAllyCharacterDataSO.CharacterSprite);
        }

        public void SetCanvasActive(bool isActive)
        {
            if (characterImage) characterImage.transform.parent.gameObject.SetActive(isActive);
        }

        #endregion

        #region Private Methods

        private void SetHexagonCharacterImage(Sprite sprite)
        {
            characterImage.sprite = sprite;
        }

        #endregion
    }
}