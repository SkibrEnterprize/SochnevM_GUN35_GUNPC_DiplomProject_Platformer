using UnityEngine;

[CreateAssetMenu(menuName = "Game Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("JumpConfiguration")]
    public int JumpForce = 500;
    public float GroundCheckDistance = 0.1f;   
}