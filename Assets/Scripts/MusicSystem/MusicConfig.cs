using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicConfig", menuName = "Configs/MusicConfig")]
public class MusicConfig : ScriptableObject
{
    [SerializeField] private List<LevelMusicData> _tracks;

    public AudioClip GetTrackByBuildIndex(int index)
    {
        return _tracks.FirstOrDefault(x => x.BuildIndex == index)?.Clip;
    }
}