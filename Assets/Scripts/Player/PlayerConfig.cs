using UnityEngine;

[CreateAssetMenu(menuName = "Game Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("Movement Configuration")]
    public float MoveSpeed = 1;
    public float MoveSpeedGround = 1;
    public float MoveSpeedAir = 0.5f;
    public float SmoothTime = 0.5f;


    [field: SerializeField, Range(0, 1)]
    public float DampAir = 0.5f;

    [field: SerializeField, Range(0, 1)]
    public float DampGround = 0.5f;


    [Header("Gravitatio Configuration")]
    public float Gravity = 0.5f;
    [field: SerializeField, Range(0, 1)]
    public float SlowClingFallSpeed = 0.5f;

    //[field: SerializeField, Range(0, 1)]    

    [Header("Jump Configuration")]
    public float JumpForce = 500;
    public float WallJumpForceX = 1f;
    public float WallJumpForceY = 0.5f;
    public float WallHorizontalPush = 2f;
    public float WallSlideSpeed = 2f;

    public int JumpCountInAir = 2;


    public float GroundCheckDistance = 0.1f;
}