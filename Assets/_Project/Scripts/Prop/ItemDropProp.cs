using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropProp : DamagableProp
{
    public GameObject dropItem;
    public bool isKinematic = false;
    protected override void Update()
    {
        if (rb.isKinematic) return;

        remainTime += Time.deltaTime;
        if (remainTime > lifeTime)
        {
            rb.isKinematic = true;
        }
    }

    public override void TakeDamage(Vector3 rayOrigin, Vector3 hitPoint, float damage)
    {
        TakeDamage(damage);

        if (hitFx)
        {
            LeanPool.Despawn(LeanPool.Spawn(hitFx, hitPoint, Quaternion.LookRotation(rayOrigin - hitPoint)), 5f);
        }
        if (!isKinematic)
        {
            BeRigidbody();
            rb.AddForce(((Random.onUnitSphere * .1f) + (hitPoint - rayOrigin).normalized) * 10f, ForceMode.Impulse);
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (currentHealth < 1 && dropItem)
        {
            Ray ray = new Ray(transform.position + Vector3.up * .1f, Vector3.down);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("Ground")))
            {
                LeanPool.Spawn(dropItem, hit.point, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
