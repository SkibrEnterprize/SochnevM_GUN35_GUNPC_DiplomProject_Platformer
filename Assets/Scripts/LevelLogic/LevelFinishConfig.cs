using UnityEngine;

[CreateAssetMenu(menuName = "Game Configs/Level Finish")]
public class LevelFinishConfig : ScriptableObject
{
    [Header("Кол-во собираемых объектов для завершения уровня")]
    [SerializeField] private int _collectObjectsForGoal = 3;

    public int CollectObjectsForGoal => _collectObjectsForGoal;
}