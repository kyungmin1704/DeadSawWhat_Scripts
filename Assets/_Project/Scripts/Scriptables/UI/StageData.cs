using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage",menuName ="Scriptable Object/Stage")]
public class StageData : ScriptableObject
{
    public StageWavesData waves;
    public string stageName;
    public int sceneIndex;
    public Sprite stageSprite;
    [TextArea(3,3)]
    public string description;
}
