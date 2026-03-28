using UnityEngine;

[CreateAssetMenu(menuName = "Game Configs/CombatConfig")]

public class CombatConfig : ScriptableObject
{
    public AttackData LightAttack;
    public AttackData HeavyAttack;
    public LayerMask EnemyLayer; // Слой врагов
}