using DG.Tweening;
using Lean.Pool;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyDead : BaseState
{
    private EnemyFSM fsm;
    
    [Tooltip("you can allocate DropItem to null for chance of drop nothing")]
    public RandomDrop[] randomDrops;
    public GameObject deadFX;

    public override void Init(FiniteStateMachine fsm)
    {
        this.fsm = fsm as EnemyFSM;
    }

    public override void Enter()
    {
        int maxWeight = 0;
        foreach (RandomDrop i in randomDrops)
        {
            maxWeight += i.weight;
        }
        int ranWeight = Random.Range(0, maxWeight);
        int curWeight = 0;
        foreach (RandomDrop i in randomDrops)
        {
            curWeight += i.weight;
            if (curWeight > ranWeight)
            {
                if (i.dropItem) LeanPool.Spawn(i.dropItem, transform.position + Vector3.up * .5f, Quaternion.identity);
                break;
            }
        }
        fsm.Anima.SetTrigger("IsDeath");
        fsm.Col.enabled = false;
    }

    public override void UpdateLogic()
    {
        
    }

    public override void UpdatePhysics()
    {
        
    }

    public override void Exit()
    {
        
    }

    public void Death()
    {
        GameObject obj = LeanPool.Spawn(deadFX, transform.position, Quaternion.identity);
        obj.transform.localScale = Vector3.one;
        LeanPool.Despawn(obj, 5f);

        SpawnManager.Instance.Despawn = gameObject;
    }
    
    [Serializable]
    public class RandomDrop
    {
        public GameObject dropItem;
        public int weight;
    }
}
