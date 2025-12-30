using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpAttack : EnemyAttack
{
    public float attackInterval;
    protected float attackCooldown;
    public GameObject attackFX;

    protected void Update()
    {
        if (attackCooldown > -1)
            attackCooldown -= Time.deltaTime;
    }

    public override void Enter()
    {
        fsm.Anima.SetTrigger("IsJumpAttack");
        attackCooldown = attackInterval;
    }

    public override void Attack()
    {
        ray.origin = transform.position + fsm.CenterOffset;
        ray.direction = transform.forward;
        hits = Physics.SphereCastAll(ray, attackHitSphereSize, 0f, LayerMask.GetMask("Player", "Prop"));
        GameObject obj = LeanPool.Spawn(attackFX, ray.origin, Quaternion.identity);
        obj.transform.localScale = Vector3.one * attackHitSphereSize;
        LeanPool.Despawn(obj, 3f);

        foreach (RaycastHit i in hits)
        {
            IDamageable target = i.collider.GetComponent<IDamageable>();
            target.TakeDamage(ray.origin, i.point, damage);
            target.PlayDamageEffect("MeleeHit", 1f);
        }
    }

    public override bool CanAttackPlayer()
    {
        if (attackCooldown > 0) return false;
        return base.CanAttackPlayer();
    }
}
