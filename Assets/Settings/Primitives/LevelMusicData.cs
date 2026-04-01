using System;
using UnityEngine;

[Serializable]
public class LevelMusicData
{
    public int BuildIndex;
    public AudioClip Clip;
    [Tooltip("Просто комментарий для удобства")]
    public string Description;
}