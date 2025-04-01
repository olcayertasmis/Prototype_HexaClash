using DG.Tweening;
using UnityEngine;

namespace HexaClash.Game.Scripts.Gameplay.Environment
{
    [RequireComponent(typeof(Collider))] [RequireComponent(typeof(Rigidbody))]
    public class InteractiveTree : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform shakingPart;
        [SerializeField] private Collider boxCollider;


        [Header("Shake Settings")]
        [SerializeField] private float shakeDuration;
        [SerializeField] private float shakeStrength;
        [SerializeField] private int shakeVibrato;
        [SerializeField] private float shakeRandomness;
        [SerializeField] private bool fadeOut;

        [Header("Cooldown")]
        [SerializeField] private float shakeCooldown;

        [Header("Controls")]
        private bool _canShake = true;
        private Tween _currentShakeTween;
        private Vector3 _originalPosition;

        private void Awake()
        {
            _originalPosition = shakingPart.localPosition;

            if (!boxCollider) boxCollider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_canShake || !other.CompareTag("Enemy")) return;

            ShakeTree();
        }

        private void ShakeTree()
        {
            if (!_canShake) return;

            _canShake = false;
            _currentShakeTween?.Kill();

            shakingPart.localRotation = Quaternion.identity;

            _currentShakeTween = shakingPart.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, fadeOut).OnComplete(() =>
            {
                shakingPart.localPosition = _originalPosition;
                StartCooldown();
            });
        }

        private void StartCooldown() => Invoke(nameof(ResetCooldown), shakeCooldown);
        private void ResetCooldown() => _canShake = true;
        private void OnDestroy() => _currentShakeTween?.Kill();
    }
}