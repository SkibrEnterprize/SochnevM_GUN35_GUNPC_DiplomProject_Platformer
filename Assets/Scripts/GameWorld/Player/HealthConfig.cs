using UnityEngine;

[CreateAssetMenu(menuName = "Configs/PlayerHealth")]
public sealed class HealthConfig : ScriptableObject
{
    [Header("General")]
    public int MaxHealth = 100;            

    [Header("Fall damage")]
    public float DamageofFall = 10f;
    public float MinHeightForDamage = 5f;   
}