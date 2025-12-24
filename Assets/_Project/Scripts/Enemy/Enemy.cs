using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Lean.Pool;
using Photon.Pun;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IDamageable, IInitializable
{
    public float maxHealth;
    public float currentHealth;
    public float damage;
    public float attackRadius;
    public float attackHitSphereSize;
    public float attackHitRadius;
    public float knockBackForce;
    public GameObject hitFX;
    public GameObject mustItem;
    public GameObject randomItem;
    
    public GameObject deathFX;

    public float sfxInterval;
    public SfxChannelPlayer approchSfx;
    
    private float sfxCooldown;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Collider col;
    private Animator anima;
    
    private EnemyState state;
    protected EnemyState State
    {
        get => state;
        set
        {
            OnEndState(state);
            state = value;
            OnBeginState(state);
        }
    }

    private Ray ray;
    private RaycastHit hit;
    protected Vector3 centerOffset;

    public void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        anima = GetComponentInChildren<Animator>();

        approchSfx.Init();
        sfxCooldown = Random.Range(sfxInterval/2, sfxInterval);
        
        col.enabled = true;
        ray = new Ray();
        centerOffset = new Vector3(0, agent.height / 2, 0);

        State = EnemyState.Chase;
        
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        OnState(state);
    }

    private void OnBeginState(EnemyState stateParm)
    {
        switch (stateParm)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Chase:
                agent.enabled = true;
                if (agent.enabled) agent.isStopped = false;
                anima.SetTrigger("IsRun");
                break;
            case EnemyState.Attack:
                anima.SetTrigger("IsAttack");
                break;
            case EnemyState.Damaged:
                agent.enabled = false;
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
                rb.AddForce(-transform.forward * 1f, ForceMode.Impulse);
                anima.SetTrigger("IsTakeDamage");
                break;
            case EnemyState.Dead:
                Vector3 here = transform.position + Vector3.up * 0.5f;
                if (mustItem != null)
                {
                    GameObject meat = LeanPool.Spawn(mustItem, here, Quaternion.identity);
                }
                if (randomItem != null && Random.value < 0.5f)
                {
                    LeanPool.Spawn(randomItem, here, Quaternion.identity);
                }
                anima.SetTrigger("IsDeath");
                col.enabled = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stateParm), stateParm, null);
        }
    }

    private void OnState(EnemyState stateParm)
    {
        switch (stateParm)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Chase:
                agent.SetDestination(GameManager.Instance.player.transform.position);
                if (CanAttackPlayer())
                {
                    State = EnemyState.Attack;
                }

                sfxCooldown -= Time.deltaTime;
                if (sfxCooldown < 0)
                {
                    sfxCooldown = sfxInterval;
                    approchSfx.PlaySfx();
                }
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Damaged:
                break;
            case EnemyState.Dead:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stateParm), stateParm, null);
        }
    }

    private void OnEndState(EnemyState stateParm)
    {
        switch (stateParm)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Chase:
                agent.isStopped = true;
                agent.enabled = false;
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Damaged:
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                break;
            case EnemyState.Dead:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stateParm), stateParm, null);
        }
    }

    public void TakeDamage(Vector3 rayOrigin, Vector3 hitPoint, float damage)
    {
        currentHealth -= damage;
        Vector3 knockBackDir = hitPoint - rayOrigin;
        knockBackDir.y = 0;
        knockBackDir.Normalize();
        transform.position += knockBackDir * knockBackForce;
        LeanPool.Despawn(LeanPool.Spawn(hitFX, hit.point, Quaternion.LookRotation((hitPoint - rayOrigin).normalized)), 2f);

        if (currentHealth <= 0) State = EnemyState.Dead;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0) State = EnemyState.Dead;
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

    public virtual void Attack()
    {
        Ray atkRay = new Ray(transform.position + centerOffset + transform.forward, transform.forward);
        RaycastHit[] atkHits = Physics.SphereCastAll(atkRay, attackHitSphereSize, 0f, LayerMask.GetMask("Player", "Prop")); 
        foreach (var i in atkHits)
        {
            IDamageable target = i.collider.GetComponent<IDamageable>();
            target.TakeDamage(atkRay.origin, i.point, damage);
            target.PlayDamageEffect("MeleeHit", 1f);
        }
    }

    public void AttackEnd()
    {
        ray.origin = transform.position + centerOffset;
        ray.direction = transform.forward;

        if (State == EnemyState.Dead) return;

        if (!Physics.Raycast(ray, out hit, attackRadius, LayerMask.GetMask("Player", "Prop")))
        {
            State = EnemyState.Chase;
        }
    }

    public void KnockBackEnd()
    {
        State = EnemyState.Chase;
    }

    public void Death()
    {
        GameObject obj = LeanPool.Spawn(deathFX, transform.position, Quaternion.identity);
        obj.transform.localScale = Vector3.one;
        LeanPool.Despawn(obj, 5f);
        SpawnManager.Instance.Despawn = gameObject;
    }
}

public enum EnemyState
{
    Idle,
    Chase,
    Attack,
    Damaged,
    Dead,
}
