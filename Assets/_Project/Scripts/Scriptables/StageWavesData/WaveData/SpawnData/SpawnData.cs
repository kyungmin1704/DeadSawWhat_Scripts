using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Scriptables/SpawnData")]
public class SpawnData : ScriptableObject
{
    public EnemyType type;
    public float healthMultiplier;
    public int count;
    public float interval;
}
