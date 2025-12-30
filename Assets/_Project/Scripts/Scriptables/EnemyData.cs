using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptables/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyType type;
    public GameObject enemyPrefab;

}
