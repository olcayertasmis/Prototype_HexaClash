using DG.Tweening;
using HexaClash.Game.Scripts.Managers;
using UnityEngine;

namespace HexaClash.Game.Scripts.Gameplay.Battle
{
    public class BattleCameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera mainCamera;

        [Header("Camera Move Animation Settings")]
        [SerializeField] private float battleZOffset;
        [SerializeField] private float moveDuration;

        private Vector3 _originalPosition;

        #region Unity Methods

        private void Awake()
        {
            _originalPosition = mainCamera.transform.position;
        }

        private void Start()
        {
            var battleManager = GameManager.Instance.GetSystem<BattleManager>();
            battleManager.OnBattleStart += OnBattleStart;
            battleManager.OnBattleEnd += OnBattleEnd;
        }

        private void OnDestroy()
        {
            var battleManager = GameManager.Instance.GetSystem<BattleManager>();
            battleManager.OnBattleStart -= OnBattleStart;
            battleManager.OnBattleEnd -= OnBattleEnd;
        }

        #endregion

        #region Action Subscribe Methods

        private void OnBattleStart()
        {
            MoveToBattlePosition();
        }

        private void OnBattleEnd()
        {
            ReturnToOriginalPosition();
        }

        #endregion

        #region Private Methods

        private void MoveToBattlePosition()
        {
            Vector3 targetPos = _originalPosition + new Vector3(0, 0, battleZOffset);
            mainCamera.transform.DOMove(targetPos, moveDuration);
        }

        private void ReturnToOriginalPosition()
        {
            mainCamera.transform.DOMove(_originalPosition, moveDuration);
        }

        #endregion
    }
}