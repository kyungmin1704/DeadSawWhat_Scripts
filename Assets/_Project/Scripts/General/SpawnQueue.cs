
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnQueue
{
    public EnemyType Type { get; private set; }
    public float HealthMulti { get; private set; }

    public SpawnQueue(EnemyType type, float healthMulti)
    {
        Type = type;
        HealthMulti = healthMulti;
    }
}

public enum EnemyType
{
    Default = 0,
    Ranged = 1,
    Boss = 2,
    SelfExplode = 3,
}
