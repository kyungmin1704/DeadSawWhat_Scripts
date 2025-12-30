using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRanged : Enemy
{
    public GameObject attackFX;

    protected override bool CanAttackPlayer()
    {
        if (Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < attackRadius)
            return true;
        else
            return false;
    }

    public override void Attack()
    {
        Ray atkRay = new Ray(transform.position + centerOffset, transform.forward);
        GameObject obj = LeanPool.Spawn(attackFX, transform.position + centerOffset, Quaternion.identity);
        obj.transform.localScale = Vector3.one * attackHitSphereSize * 1.2f;
        LeanPool.Despawn(obj, 3);
        float targetDist = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);
        targetDist = targetDist < 1f ? 1f : targetDist;
        if (targetDist < attackHitSphereSize)
        {
            GameManager.Instance.player.TakeDamage(damage / targetDist);
            GameManager.Instance.player.PlayDamageEffect("Interrupt", 1 / targetDist);
        }
    }
}
