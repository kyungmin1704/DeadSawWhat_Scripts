using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAttack : BaseState
{
    protected EnemyFSM fsm;

    public float damage;
    public float attackRadius;
    public float attackHitSphereSize;
    public string sfxName;

    protected Ray ray;
    protected RaycastHit[] hits;
    protected SfxChannelPlayer attackSfx;
    
    public override void Init(FiniteStateMachine fsm)
    {
        this.fsm = fsm as EnemyFSM;
        ray = new Ray();
        attackSfx = !attackSfx ? GetComponentsInChildren<SfxChannelPlayer>().ToList().Find(obj => obj.sfxName == sfxName).Init() : attackSfx;
    }

    public override void Enter()
    {
        fsm.Anima.SetTrigger("IsAttack");
        attackSfx?.PlaySfx();
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

    public virtual void Attack()
    {
        ray.origin = transform.position + fsm.CenterOffset + transform.forward;
        ray.direction = transform.forward;
        hits = Physics.SphereCastAll(ray, attackHitSphereSize, 0f, LayerMask.GetMask("Player", "Prop"));
        
        foreach (RaycastHit i in hits)
        {
            IDamageable target = i.collider.GetComponent<IDamageable>();
            target.TakeDamage(ray.origin, i.point, damage);
            target.PlayDamageEffect("MeleeHit", 1f);
        }
    }

    public virtual void AttackEnd()
    {
        if (fsm.CurrentState == fsm.deadState) return;

        if (!CanAttackPlayer())
        {
            fsm.CurrentState = fsm.chaseState;
        }
    }

    public virtual bool CanAttackPlayer()
    {
        ray.origin = transform.position + fsm.CenterOffset;
        ray.direction = transform.forward;
        return Physics.Raycast(ray, out RaycastHit hit, attackRadius, LayerMask.GetMask("Player", "Prop"));
    }
}
