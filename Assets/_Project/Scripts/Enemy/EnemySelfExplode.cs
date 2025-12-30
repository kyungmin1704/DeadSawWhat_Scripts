using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySelfExplode : Enemy
{
    public GameObject atkFX;
    public override void Attack()
    {
        Ray atkRay = new Ray(transform.position + centerOffset, transform.forward);
        RaycastHit[] atkHits = Physics.SphereCastAll(atkRay, attackHitSphereSize, 0f, LayerMask.GetMask("Player"));
        GameObject obj = LeanPool.Spawn(atkFX, transform.position + centerOffset, Quaternion.identity);
        obj.transform.localScale = Vector3.one * attackHitSphereSize;
        LeanPool.Despawn(obj, 3f);
        foreach (RaycastHit i in atkHits)
        {
            IDamageable target = i.collider.GetComponent<IDamageable>();
            target.TakeDamage(atkRay.origin, i.point, damage);
            target.PlayDamageEffect("MeleeHit", 1f);
        }

        SpawnManager.Instance.Despawn = gameObject;
    }
}