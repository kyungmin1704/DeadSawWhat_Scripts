using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : EnemyAttack
{
    public GameObject attackFX;

    public override void Enter()
    {
        fsm.Anima.SetTrigger("IsAttack");
    }

    public override void Attack()
    {
        attackSfx?.PlaySfx();
        ray.origin = transform.position + fsm.CenterOffset;
        ray.direction = transform.forward;
        hits = Physics.SphereCastAll(ray, attackHitSphereSize, 0f, LayerMask.GetMask("Player"));
        GameObject obj = LeanPool.Spawn(attackFX, ray.origin, Quaternion.identity);
        obj.transform.localScale = Vector3.one * attackHitSphereSize * 1.2f;
        LeanPool.Despawn(obj, 5f);

        foreach (RaycastHit i in hits)
        {
            float targetDist = Vector3.Distance(ray.origin, i.collider.transform.position);
            targetDist = targetDist < 1f ? 1f : targetDist;
            IDamageable target = i.collider.gameObject.GetComponent<IDamageable>();
            target.TakeDamage(ray.origin, i.point, damage / targetDist);
            target.PlayDamageEffect("Interrupt", 1f / targetDist);
        }
    }

    public override bool CanAttackPlayer()
    {
        ray.origin = transform.position + fsm.CenterOffset;
        ray.direction = transform.forward;
        hits = Physics.SphereCastAll(ray, attackHitSphereSize, 0f, LayerMask.GetMask("Player"));
        
        return hits.Length > 0;
    }
}
