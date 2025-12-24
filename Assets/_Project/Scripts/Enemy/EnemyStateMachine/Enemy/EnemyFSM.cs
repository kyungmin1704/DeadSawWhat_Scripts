using Lean.Pool;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : FiniteStateMachine, IDamageable, IInitializable, IPunInstantiateMagicCallback
{
    public float knockBackForce;
    
    [SerializeField] private float maxHealth;
    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value > 10 ? value : 10;
    }
    
    private float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value > maxHealth ? maxHealth : value;
            if (currentHealth <= 0) OnDead();
        }
    }

    public GameObject hitFX;

    public EnemyChase chaseState;
    public EnemyAttack[] attackStates;
    public EnemyDead deadState;
    
    public NavMeshAgent Agent { get; private set; }
    public Rigidbody Rb { get; private set; }
    public Collider Col { get; private set; }
    public Animator Anima { get; private set; }
    public PhotonView Pv { get; private set; }
    

    public Vector3 CenterOffset { get; private set; }

    public void Init()
    {
        Agent = !Agent ? GetComponent<NavMeshAgent>() : Agent;
        Rb = !Rb ? Agent.GetComponent<Rigidbody>() : Rb;
        Col = !Col ? GetComponent<Collider>() : Col;
        Anima = !Anima ? GetComponentInChildren<Animator>() : Anima;
        Pv = !Pv ? GetComponent<PhotonView>() : Pv;
        
        chaseState.Init(this);
        foreach (var i in attackStates)
        {
            i.Init(this);
        }
        deadState.Init(this);

        Col.enabled = true;
        CenterOffset = new Vector3(0, Agent.height / 2, 0);

        CurrentState = defaultState;
        CurrentHealth = MaxHealth;
    }

    [PunRPC]
    public void OnDead()
    {
        CurrentState = deadState;
        if (Pv && Pv.IsMine)
            Pv.RPC("OnDead", RpcTarget.Others);
    }

    [PunRPC]
    public void TakeDamageEffect(Vector3 rayOrigin, Vector3 hitPoint)
    {
        LeanPool.Despawn(
            LeanPool.Spawn(hitFX, hitPoint, Quaternion.LookRotation((hitPoint - rayOrigin).normalized)), 3f);
    }

    [PunRPC]
    public void TakeDamage(Vector3 rayOrigin, Vector3 hitPoint, float damage)
    {
        TakeDamageEffect(rayOrigin, hitPoint);
        if (!Pv)
        {
            Vector3 knockBackDir = hitPoint - rayOrigin;
            knockBackDir.y = 0;
            knockBackDir.Normalize();
            transform.position += knockBackDir * knockBackForce;
            TakeDamage(damage);
        }
        else if (Pv.IsMine)
        {
            Vector3 knockBackDir = hitPoint - rayOrigin;
            knockBackDir.y = 0;
            knockBackDir.Normalize();
            transform.position += knockBackDir * knockBackForce;
            TakeDamage(damage);
            Pv.RPC("TakeDamageEffect", RpcTarget.Others, rayOrigin, hitPoint);
        }
        else
        {
            Pv.RPC("TakeDamage", RpcTarget.MasterClient, rayOrigin, hitPoint, damage);
        }
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
    }

    public void PlayDamageEffect(string effectName, float cameraShakeWeight)
    {
        throw new System.NotImplementedException();
    }

    public void Death()
    {
        if (CurrentState == deadState) deadState.Death();
    }

    public void Attack()
    {
        if (attackStates.Contains(CurrentState)) (CurrentState as EnemyAttack).Attack();
    }

    public void AttackEnd()
    {
        if (attackStates.Contains(CurrentState)) (CurrentState as EnemyAttack).AttackEnd();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (!info.photonView.IsMine)
            Init();
    }
}
