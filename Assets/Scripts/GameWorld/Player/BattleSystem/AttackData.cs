using System;

[Serializable]
public struct AttackData
{
    public int Damage;
    public float Range;
    public float Cooldown;
    public float KnockbackForce; // Сила отталкивания
}