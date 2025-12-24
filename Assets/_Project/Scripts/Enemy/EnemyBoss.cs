using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : StateMachine<BossState>, IDamageable, IInitializable
{
    
    public float maxHealth;
    public float currentHealth;
    public float damage;
    public float attackRadius;
    public float attackHitSphereSize;
    public float attackHitRadius;
    public float knockBackForce;
    public GameObject hitFX;
    public GameObject deathFX;

    public float pattern0Damage;
    public float pattern0HitRadius;
    public GameObject pattern0FX;
    
    [SerializeField] private float pattern0Interval;
    private float pattern0cooldown;
    
    private UnityEngine.AI.NavMeshAgent agent;
    private Rigidbody rb;
    private Collider col;
    private Animator anima;

    private Ray ray;
    private RaycastHit hit;
    private RaycastHit[] atkHits;
    protected Vector3 centerOffset;

    public void Init()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        anima = GetComponentInChildren<Animator>();

        col.enabled = true;
        
        ray = new Ray();
        centerOffset = new Vector3(0, agent.height / 2, 0);

        State = BossState.Chase;
        
        currentHealth = maxHealth;
    }

    protected override void Update()
    {
        base.Update();
        
        if (pattern0cooldown > 0) pattern0cooldown -= Time.deltaTime;
        else pattern0cooldown = 0;
    }

    protected override void OnBeginState(BossState state)
    {
        switch (state)
        {
            case BossState.Idle:
                break;
            case BossState.Chase:
                agent.enabled = true;
                if (agent.enabled) agent.isStopped = false;
                anima.SetTrigger("IsRun");
                break;
            case BossState.Attack:
                anima.SetTrigger("IsAttack");
                break;
            case BossState.Damaged:
                agent.enabled = false;
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
                rb.AddForce(-transform.forward * 1f, ForceMode.Impulse);
                anima.SetTrigger("IsTakeDamage");
                break;
            case BossState.Dead:
                anima.SetTrigger("IsDeath");
                break;
            case BossState.Pattern0:
                anima.SetTrigger("Pattern0");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    protected override void OnState(BossState state)
    {
        switch (state)
        {
            case BossState.Idle:
                break;
            case BossState.Chase:
                agent.SetDestination(GameManager.Instance.player.transform.position);
                if (CanPattern0())
                {
                    State = BossState.Pattern0;
                    pattern0cooldown = pattern0Interval;
                    break;
                }
                
                if (CanAttackPlayer())
                {
                    State = BossState.Attack;
                    break;
                }
                break;
            case BossState.Attack:
                break;
            case BossState.Damaged:
                break;
            case BossState.Dead:
                break;
            case BossState.Pattern0:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    protected override void OnEndState(BossState state)
    {
        switch (state)
        {
            case BossState.Idle:
                break;
            case BossState.Chase:
                agent.isStopped = true;
                agent.enabled = false;
                break;
            case BossState.Attack:
                break;
            case BossState.Damaged:
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                break;
            case BossState.Dead:
                break;
            case BossState.Pattern0:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void TakeDamage(Vector3 rayOrigin, Vector3 hitPoint, float damage)
    {
        currentHealth -= damage;
        Vector3 knockBackDir = hitPoint - rayOrigin;
        knockBackDir.y = 0;
        knockBackDir.Normalize();
        transform.position += knockBackDir * knockBackForce;
        Destroy(Instantiate(hitFX, hit.point, Quaternion.LookRotation((hitPoint - rayOrigin).normalized)), 2f);

        if (currentHealth <= 0) State = BossState.Dead;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0) State = BossState.Dead;
    }

    public void PlayDamageEffect(string effectName, float cameraShakeWeight)
    {
        throw new NotImplementedException();
    }

    protected virtual bool CanAttackPlayer()
    {
        ray.origin = transform.position + centerOffset;
        ray.direction = transform.forward;
        if (Physics.Raycast(ray, out hit, attackRadius, LayerMask.GetMask("Player", "Prop"))) 
            return true;
        else
            return false;
    }

    protected bool CanPattern0()
    {
        if (pattern0cooldown > 0) return false;
        ray.origin = transform.position + centerOffset;
        ray.direction = transform.forward;
        return Physics.Raycast(ray, out hit, attackRadius, LayerMask.GetMask("Player","Prop"));
    }

    public virtual void Attack()
    {
        Ray atkRay = new Ray(transform.position + centerOffset + transform.forward, transform.forward);
        atkHits = Physics.SphereCastAll(atkRay, attackHitSphereSize, 0f, LayerMask.GetMask("Player", "Prop"));
        foreach (var i in atkHits)
        {
            IDamageable target = i.collider.GetComponent<IDamageable>();
            target.TakeDamage(atkRay.origin, i.point, damage);
            target.PlayDamageEffect("MeleeHit", 1f);
        }
    }

    public void Pattern0Attack()
    {
        LeanPool.Despawn(LeanPool.Spawn(pattern0FX, transform.position, Quaternion.identity), 3f);
        
        Ray atkRay = new Ray(transform.position + centerOffset, transform.forward);
        RaycastHit[] atkHit = Physics.SphereCastAll(atkRay, pattern0HitRadius, 0f, LayerMask.GetMask("Player", "Prop"));
        foreach (var i in atkHit)
        {
            IDamageable target = i.collider.GetComponent<IDamageable>();
            target.TakeDamage(atkRay.origin, i.point, pattern0Damage);
            target.PlayDamageEffect("MeleeHit", 1f);
        }
    }

    public void AttackEnd()
    {
        ray.origin = transform.position + centerOffset;
        ray.direction = transform.forward;
        if (!Physics.Raycast(ray, out hit, attackRadius, LayerMask.GetMask("Player", "Prop")))
        {
            State = BossState.Chase;
        }
    }

    public void Pattern0End()
    {
        State = BossState.Chase;
    }

    public void KnockBackEnd()
    {
        State = BossState.Chase;
    }

    public void Death()
    {
        SpawnManager.Instance.Despawn = gameObject;
        GameObject obj = LeanPool.Spawn(deathFX, transform.position, Quaternion.identity);
        obj.transform.localScale = Vector3.one * 1.2f;
        LeanPool.Despawn(obj, 5f);
    }
}

public enum BossState
{
    Idle,
    Chase,
    Attack,
    Damaged,
    Dead,
    Pattern0,
}
