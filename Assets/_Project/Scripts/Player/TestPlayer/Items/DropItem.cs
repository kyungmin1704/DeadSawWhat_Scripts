using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    Coroutine routine;
    void OnEnable()
    {
        routine = StartCoroutine(GoAway());
    }

    void OnDisable()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    IEnumerator GoAway()
    {
        yield return new WaitForSeconds(10f);

        if (gameObject.activeInHierarchy)
            LeanPool.Despawn(gameObject);
    }
}
