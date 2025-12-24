using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventHandler : MonoBehaviour
{
    public Enemy enemy;
    
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

    void Death()
    {
        enemy.Death();
    }
}
