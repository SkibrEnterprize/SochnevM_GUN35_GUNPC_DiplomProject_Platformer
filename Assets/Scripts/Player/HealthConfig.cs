using UnityEngine;

[CreateAssetMenu(menuName = "Configs/PlayerHealth")]
public sealed class HealthConfig : ScriptableObject
{
    [Header("General")]
    public int MaxHealth = 100;               // максимальное здоровье

    [Header("Fall damage")]
    public float DamageofFall = 10f; // сколько урона при падении
    public float MinHeightForDamage = 5f;   // ниже этой скорости не будет уронов
}