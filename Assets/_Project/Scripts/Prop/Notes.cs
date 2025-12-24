using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Notes : MonoBehaviour, IDamageable
{
    public Transform point;
    public GameObject notes;
    bool isActivate;
    void Start()
    {
        isActivate = false;
        if (point == null)
        {
            point = transform;
        }
    }

    void Update()
    {
        if (!isActivate) {return;}
        else if (isActivate)
        {
            Party();
        }
    }
    public void TakeDamage(Vector3 rayOrigin, Vector3 hitPoint, float damage)
    {
        isActivate = true;
    }

    public void TakeDamage(float damage)
    {
        isActivate = true;
    }

    public void PlayDamageEffect(string effectName, float cameraShakeWeight)
    {

    }

    void Party()
    {
        LeanPool.Despawn(LeanPool.Spawn(notes, point), 2f);
        isActivate = false;
    }
}
