using UnityEngine;
using Zenject;
using System.Threading.Tasks;

public class PlayerAnimator
{
    private readonly Animator _animator;
    private readonly SkinnedMeshRenderer _meshRenderer;
    private MaterialPropertyBlock _propBlock;

    // Хеши параметров для оптимизации (быстрее, чем строки)
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int IsWallSlidingHash = Animator.StringToHash("IsWallSliding");
    private static readonly int IsFlyingHash = Animator.StringToHash("IsFlying");

    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int HeavyAttackTrigger = Animator.StringToHash("HeavyAttack");
    private static readonly int HitTrigger = Animator.StringToHash("Hit");
    private static readonly int DieTrigger = Animator.StringToHash("Die");
    private static readonly int IsDeadBool = Animator.StringToHash("IsDead");
    private static readonly int LandingTrigger = Animator.StringToHash("Landing");

    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private bool _isDead;
    private bool _isHurt;

    public PlayerAnimator(Animator animator, SkinnedMeshRenderer meshRenderer)
    {
        _animator = animator;
        _meshRenderer = meshRenderer;
        _propBlock = new MaterialPropertyBlock();
    }

    /// <summary>
    /// Основной метод обновления анимаций перемещения. 
    /// Вызывается из PlayerMovementSystem.ApplyAnimation()
    /// </summary>
    public void UpdateMovementStates(float normalizedSpeed, bool isGrounded, bool isWallSliding, bool isFlying)
    {
        if (_isDead) return;

        float finalSpeed = _isHurt ? 0f : normalizedSpeed;

        _animator.SetFloat(SpeedHash, finalSpeed);
        _animator.SetBool(IsGroundedHash, isGrounded);
        _animator.SetBool(IsWallSlidingHash, isWallSliding);
        _animator.SetBool(IsFlyingHash, isFlying);        
    }

    public void PlayLanding()
    {
        if (_isDead || _isHurt) return;
        _animator.SetTrigger(LandingTrigger);
    }
    public void PlayAttack(bool isHeavy)
    {
        if (_isDead) return;
        _animator.SetTrigger(isHeavy ? HeavyAttackTrigger : AttackTrigger);
    }

    public void PlayHit(bool playAnimation = true)
    {
        if (_isDead) return;

        _ = FlashRoutine();

        if (playAnimation)
        {
            _animator.SetTrigger(HitTrigger);
            _ = HurtLockRoutine();
        }
    }

    public void PlayDeath()
    {
        if (_isDead) return;

        _isDead = true;
        _animator.SetBool(IsDeadBool, true);
        _animator.SetFloat(SpeedHash, 0f);
        _animator.SetTrigger(DieTrigger);

        _animator.ResetTrigger(HitTrigger);
        _animator.ResetTrigger(AttackTrigger);
    }

    public void ResetToIdle()
    {
        _isDead = false;
        _animator.SetBool(IsDeadBool, false);
        _animator.Play("Grounded", 0, 0f); 
        _animator.ResetTrigger(DieTrigger);
    }

    private async Task FlashRoutine()
    {
        Debug.Log("Flash Started");
        if (_meshRenderer == null) return;

        _meshRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(ColorId, Color.red);
        _meshRenderer.SetPropertyBlock(_propBlock);

        await Task.Delay(120);

        if (_meshRenderer != null)
        {
            _meshRenderer.GetPropertyBlock(_propBlock);
            _propBlock.Clear();
            _meshRenderer.SetPropertyBlock(_propBlock);
        }
    }

    private async Task HurtLockRoutine()
    {
        _isHurt = true;
        await Task.Delay(400);
        _isHurt = false;
        _animator.ResetTrigger(HitTrigger);
    }
}
    