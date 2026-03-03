using UnityEngine;

[CreateAssetMenu(menuName = "Game Configs/LevelFinishConfig")]
public class LevelFinishConfig : ScriptableObject
{
    [Header("Number Of Collect Objects")]
    public int CollectObjectForGoal = 3;

}