using UnityEngine;

[CreateAssetMenu(menuName = "Game Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("JumpConfiguration")]
    public float MoveSpeedGround = 1;
    public float MoveSpeedAir = 0.5f;
    //[field: SerializeField, Range(0, 1)]
    

    public int JumpForce = 500;
    public float GroundCheckDistance = 0.1f;   
}