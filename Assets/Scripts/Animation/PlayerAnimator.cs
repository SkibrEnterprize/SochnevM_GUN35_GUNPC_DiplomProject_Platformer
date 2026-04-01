using UnityEngine;
using Zenject;

public class PlayerAnimator : ITickable
{
    private readonly Animator _animator;
    private readonly CharacterController _controller;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int HeavyAttackTrigger = Animator.StringToHash("HeavyAttack");
    private static readonly int HitTrigger = Animator.StringToHash("Hit");
    private static readonly int DieTrigger = Animator.StringToHash("Die");
    private static readonly int IsWallSlidingHash = Animator.StringToHash("IsWallSliding");
    private static readonly int IsFlyingHash = Animator.StringToHash("IsFlying");

    public PlayerAnimator(Animator animator, CharacterController controller)
    {
        _animator = animator;
        _controller = controller;
    }

    public void Tick()
    {
        // 1. Передаем горизонтальную скорость (берем только X и Z)
        Vector3 horizontalMove = new Vector3(_controller.velocity.x, 0, _controller.velocity.z);
        _animator.SetFloat(Speed, horizontalMove.magnitude);

        // 2. Передаем состояние земли
        _animator.SetBool(IsGrounded, _controller.isGrounded);
    }
    public void UpdateMovementStates(bool isWallSliding, bool isFlying)
    {
        _animator.SetBool(IsWallSlidingHash, isWallSliding);
        _animator.SetBool(IsFlyingHash, isFlying);
    }

    public void PlayAttack(bool isHeavy)
    {
        if (isHeavy)
            _animator.SetTrigger(HeavyAttackTrigger);
        else
            _animator.SetTrigger(AttackTrigger);
    }

    public void PlayHit() => _animator.SetTrigger(HitTrigger);
    public void PlayDeath()
    {
        _animator.SetTrigger(DieTrigger);
    }
}