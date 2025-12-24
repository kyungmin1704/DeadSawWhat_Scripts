using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptables/WaveData")]
public class WaveData : ScriptableObject
{
    public WaveType waveType = WaveType.Default;
    public List<SpawnData> spawnList;
}

public enum WaveType
{
    Default,
    Boss,
}
