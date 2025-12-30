using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using DG.Tweening;

public class ForItem : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up * (50f * Time.deltaTime), Space.Self);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.SetParent(null);
            LeanPool.Despawn(gameObject);
        }
    }
}
