using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyListData", menuName = "Scriptables/EnemyListData")]
public class EnemyListData : ScriptableObject
{
    public List<EnemyData> enemyDatas;
}
