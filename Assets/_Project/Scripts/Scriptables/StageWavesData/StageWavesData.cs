using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageWavesData", menuName = "Scriptables/StageWavesData")]
public class StageWavesData : ScriptableObject
{
    public List<WaveData> waveList;
}
