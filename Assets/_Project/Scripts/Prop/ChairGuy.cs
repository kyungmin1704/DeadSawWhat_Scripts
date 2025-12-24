using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Lean.Pool;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(NavMeshObstacle))]
public class ChairGuy : MonoBehaviour, IDamageable
{
    public float maxHealth;
    public float currentHealth;
    Rigidbody rb;
    public GameObject hitFX;
    public GameObject burstFX;
    public GameObject owned;
    bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        isDead = false;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            if (isDead) return;
            isDead = true;
            Drop();
            Destroy(gameObject, 2f);
        }
    }

    public void TakeDamage(Vector3 rayOrigin, Vector3 hitPoint, float damage)
    {
        BeRigidbody();
        currentHealth -= damage;
        if (currentHealth <= 1)
        {
            rb.AddForce((Random.onUnitSphere) * 30f, ForceMode.Impulse);
            LeanPool.Despawn(LeanPool.Spawn(burstFX, hitPoint, Quaternion.LookRotation((hitPoint - rayOrigin).normalized)), 2f);
        }
        else
        {
            rb.AddForce(((Random.onUnitSphere * .1f) + (hitPoint - rayOrigin).normalized) * 10f, ForceMode.Impulse);
            LeanPool.Despawn(LeanPool.Spawn(hitFX, hitPoint, Quaternion.LookRotation((hitPoint - rayOrigin).normalized)), 2f);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    public void PlayDamageEffect(string effectName, float cameraShakeWeight)
    {
        
    }

    void Drop()
    {
        if (owned)
        {
            Vector3 here = transform.position + Vector3.up * 0.5f;
            LeanPool.Spawn(owned, here, Quaternion.identity);
        }
    }
    void BeRigidbody()
    {
        rb.isKinematic = false;
    }
}
