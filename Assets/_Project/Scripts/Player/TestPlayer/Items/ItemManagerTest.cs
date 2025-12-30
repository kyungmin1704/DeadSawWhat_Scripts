using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class ItemManagerTest : MonoBehaviour
{
    public GameObject[] items;
    public Transform[] spots;

    void Awake()
    {
        ItemSpawn();
        StartCoroutine(ItemSpawnRoutine());
    }

    public void ItemSpawn()
    {
        GameObject randomItem = items[Random.Range(0, items.Length)];
        Transform randomSpot = spots[Random.Range(0, spots.Length)];

        if (randomSpot.GetComponentInChildren<ForItem>() != null)
            return;

        var hereyouare = LeanPool.Spawn(randomItem, randomSpot.position, randomSpot.rotation);
        hereyouare.transform.SetParent(randomSpot, true);
    }

    IEnumerator ItemSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            ItemSpawn();
        }
    }
}
