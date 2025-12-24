using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSMEvent : MonoBehaviour
{
    public EnemyFSM fsm;

    void Death()
    {
        fsm.Death();
    }

    void Attack()
    {
        fsm.Attack();
    }

    void AttackEnd()
    {
        fsm.AttackEnd();
    }
}
