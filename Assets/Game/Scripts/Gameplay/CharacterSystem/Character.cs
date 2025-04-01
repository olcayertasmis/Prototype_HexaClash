using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using HexaClash.Game.Scripts.Data.SOData.CharacterDatasSO;
using UnityEngine;
using UnityEngine.UI;

namespace HexaClash.Game.Scripts.Gameplay.CharacterSystem
{
    public class Character : MonoBehaviour
    {
        #region SerializeField Fields

        [Header("Components")]
        [SerializeField] private SphereCollider sphereCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private Slider healthBar;

        [Header("Animation Settings")]
        [SerializeField] private float heightInMoveAnimation;
        [SerializeField] private float moveAnimationDuration;

        #endregion

        #region Private Fields

        [Header("Data")]
        protected CharacterDataSO CharacterData;
        private int _currentHealth;
        private Character _currentTarget;

        [Header("Controls")]
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;
        private bool _isAttacking;

        [Header("Animation Hash")]
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        //private static readonly int DieHash = Animator.StringToHash("Die");

        #endregion

        #region Properties

        public bool IsInitialized { get; set; }
        public bool IsDead => _currentHealth <= 0;
        private int MaxHealth => CharacterData?.Health ?? 0;
        private int AttackPower => CharacterData?.AttackPower ?? 0;
        private float AttackRange => CharacterData?.AttackRange ?? 0f;
        protected float MoveSpeed { get; set; }

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (!IsInitialized || IsDead) return;

            HandleMovement();
        }

        private void OnDestroy()
        {
            CleanupTweens();
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(CharacterDataSO characterData)
        {
            if (!characterData) return;

            SetData(characterData);
            SetupVisuals();

            UpdateHealthBar();
        }

        public void SetTarget(Character target)
        {
            if (_currentTarget && !_currentTarget.IsDead) return;

            _currentTarget = target;
        }

        #endregion

        #region Private Methods

        private void SetData(CharacterDataSO characterData)
        {
            CharacterData = characterData;
            _currentHealth = MaxHealth;
        }

        private void SetupVisuals()
        {
            if (CharacterData.CharacterSprite) spriteRenderer.sprite = CharacterData.CharacterSprite;

            if (CharacterData.AnimatorOverrideController) animator.runtimeAnimatorController = CharacterData.AnimatorOverrideController;
        }

        private void HandleMovement()
        {
            if (!_currentTarget || _currentTarget.IsDead) return;

            float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);

            if (distance <= AttackRange) StartCoroutine(AttackRoutine());
            else MoveTowardsTarget();


            /*float originalY = transform.position.y;
            _moveTween = transform.DOMoveY(originalY + heightInMoveAnimation, moveAnimationDuration)
                .OnComplete(() => { transform.DOMoveY(originalY, moveAnimationDuration).OnComplete(() => _moveTween = null); });*/
        }

        private void MoveTowardsTarget()
        {
            Vector3 targetPosition = new Vector3(_currentTarget.transform.position.x, transform.position.y, _currentTarget.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);

            PlayMovementAnimation();
        }

        private void PlayMovementAnimation()
        {
            if (_moveTween != null) return;

            Vector3 originalPosition = transform.position;

            _moveTween = transform.DOMoveY(originalPosition.y + heightInMoveAnimation, moveAnimationDuration).SetLoops(2, LoopType.Yoyo).OnComplete(() => _moveTween = null);
        }

        private IEnumerator AttackRoutine()
        {
            if (_isAttacking || !_currentTarget || _currentTarget.IsDead) yield break;

            _isAttacking = true;

            animator.SetTrigger(AttackHash);
            yield return new WaitForSeconds(GetAnimationLength());

            _currentTarget.TakeDamage(AttackPower);
            _isAttacking = false;
        }

        private float GetAnimationLength() => animator.GetCurrentAnimatorStateInfo(0).length;

        private void TakeDamage(int damage)
        {
            if (IsDead) return;

            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            UpdateHealthBar();

            if (IsDead) Die();
            else PlayDamageEffect();
        }

        private void PlayDamageEffect()
        {
            spriteRenderer.DOKill();
            spriteRenderer.DOColor(Color.red, 0.1f).OnComplete(() => spriteRenderer.DOColor(Color.white, 0.1f));
        }

        protected virtual void Die()
        {
            //animator.SetTrigger(DieHash);
            healthBar.gameObject.SetActive(false);
            transform.DORotate(new Vector3(0, 0, -90), 0.5f).SetEase(Ease.InOutQuad).OnComplete(() => { Destroy(gameObject, 1f); });
        }

        private void UpdateHealthBar()
        {
            if (!healthBar) return;

            if (IsDead) healthBar.gameObject.SetActive(false);

            healthBar.value = (float)_currentHealth / MaxHealth;
            healthBar.gameObject.SetActive(_currentHealth < MaxHealth);
        }

        private void CleanupTweens()
        {
            _moveTween?.Kill();
            spriteRenderer.DOKill();
        }

        #endregion
    }
}