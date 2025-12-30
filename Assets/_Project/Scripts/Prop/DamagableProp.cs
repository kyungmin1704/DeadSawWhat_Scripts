using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(NavMeshObstacle))]
public class DamagableProp : MonoBehaviour, IDamageable
{
    public float maxHealth;
    protected float currentHealth;
    public float lifeTime;
    protected float remainTime;
    public GameObject hitFx;
    public GameObject destroyFx;
    
    protected Rigidbody rb;


    protected virtual void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        if (rb.isKinematic) return;
        
        remainTime += Time.deltaTime;
        if (remainTime > lifeTime)
        {
            Destroy(gameObject);
            if (destroyFx)
            {
                LeanPool.Despawn(LeanPool.Spawn(destroyFx, transform.position, Quaternion.identity), 5f);
            }
        }
    }

    public virtual void TakeDamage(Vector3 rayOrigin, Vector3 hitPoint, float damage)
    {
        TakeDamage(damage);
        if (hitFx)
        {
            LeanPool.Despawn(LeanPool.Spawn(hitFx, hitPoint, Quaternion.LookRotation(rayOrigin - hitPoint)), 5f);
        }
        if (currentHealth < 1)
        {
            BeRigidbody();
            rb.AddForce(((Random.onUnitSphere * .1f) + (hitPoint - rayOrigin).normalized) * 10f, ForceMode.Impulse);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    public void PlayDamageEffect(string effectName, float cameraShakeWeight)
    {
        
    }

    protected void BeRigidbody()
    {
        remainTime = 0;
        rb.isKinematic = false;
    }
}
