using UnityEngine;
using Zenject;
using System.Threading.Tasks;

public class PlayerAnimator : ITickable
{
    private readonly Animator _animator;
    private readonly SkinnedMeshRenderer _meshRenderer;
    private MaterialPropertyBlock _propBlock;
    private static readonly int ColorId = Shader.PropertyToID("_Color");// Или "_BaseColor" для URP шейдера
    private readonly CharacterController _controller;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int HeavyAttackTrigger = Animator.StringToHash("HeavyAttack");
    private static readonly int HitTrigger = Animator.StringToHash("Hit");
    private static readonly int DieTrigger = Animator.StringToHash("Die");
    private static readonly int IsWallSlidingHash = Animator.StringToHash("IsWallSliding");
    private static readonly int IsFlyingHash = Animator.StringToHash("IsFlying");

    private bool _isDead;

    public PlayerAnimator(Animator animator, 
        CharacterController controller, 
        SkinnedMeshRenderer meshRenderer)
    {
        _animator = animator;
        _controller = controller;
        _meshRenderer = meshRenderer;
    }

    public void Tick()
    {
        if (_isDead) return;
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
    public void ResetToIdle()
    {
        _isDead = false;
        _animator.SetBool("IsDead", false);
        _animator.Play("Grounded", 0, 0f);
        _animator.ResetTrigger(DieTrigger);
    }

    public void PlayHit() => _animator.SetTrigger(HitTrigger);
    public async void PlayHitFlash() => await FlashRoutine();

    private async Task FlashRoutine()
    {
        if (_meshRenderer == null) return;
        if (_propBlock == null) _propBlock = new MaterialPropertyBlock();

        // 1. Устанавливаем КРАСНЫЙ
        _meshRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(ColorId, Color.red);
        _meshRenderer.SetPropertyBlock(_propBlock);

        await Task.Delay(120);

        if (_meshRenderer != null)
        {
            // 2. СБРАСЫВАЕМ настройки (возвращаем как было в шейдере)
            _meshRenderer.GetPropertyBlock(_propBlock);
            _propBlock.Clear(); // Очищает все изменения, возвращая оригинальный вид
            _meshRenderer.SetPropertyBlock(_propBlock);
        }
    }
    public void PlayDeath()
    {
        _isDead = true;
        _animator.SetBool("IsDead", true);
        _animator.SetFloat(Speed, 0f);
        _animator.SetTrigger(DieTrigger);
        _animator.ResetTrigger(HitTrigger);
        Debug.Log("DIE TRIGGER");
        
    }
}