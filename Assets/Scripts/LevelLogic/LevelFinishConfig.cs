using UnityEngine;

[CreateAssetMenu(menuName = "Game Configs/Level Finish")]
public class LevelFinishConfig : ScriptableObject
{
    [Header("Кол-во собираемых объектов для завершения уровня")]
    [SerializeField] private int _collectObjectsForGoal = 3;

    [Header("Имя следующей сцены (точно как в Build Settings)")]
    [SerializeField] private string _nextSceneName;
    public int CollectObjectsForGoal => _collectObjectsForGoal;
    public string NextSceneName => _nextSceneName;


}