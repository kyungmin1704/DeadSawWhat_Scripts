using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossAnimationEventHandler : MonoBehaviour
{
    public EnemyBoss enemy;
    
    void Attack()
    {
        enemy.Attack();
    }

    void AttackEnd()
    {
        enemy.AttackEnd();
    }

    void KnockBackEnd()
    {
        enemy.KnockBackEnd();
    }

    void Pattern0Attack()
    {
        enemy.Pattern0Attack();
    }

    void Pattern0End()
    {
        enemy.Pattern0End();
    }

    void Death()
    {
        enemy.Death();
    }
}
