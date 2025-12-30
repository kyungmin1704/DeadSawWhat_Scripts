using DG.Tweening;
using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOutfitController : MonoBehaviour
{
    public GameObject gunFirePrefab;
    public GameObject tracerPrefab;
    public Transform shotPoint;

    public float walk = 0.1f;
    public float run = 1.0f;
    
    private TestPlayerController pc;

    float checkInterval = .2f;
    float checkTime;
    Animator anim;
    Vector3 previousPosition;
    Vector3 lookVector;
    
    public PlayerOutfitController Init(TestPlayerController pc)
    {
        this.pc = pc;
        anim = GetComponent<Animator>();
        previousPosition = transform.position;
        return this;
    }
    
    public void Update()
    {
        checkTime -= Time.deltaTime;
        if (checkTime > 0) return;
        
        checkTime = checkInterval;
        Vector3 movement = pc.transform.position - previousPosition;
        float distance = movement.magnitude;
        float speed = distance / Time.deltaTime;

        if (speed < walk)
        {
            anim.SetBool("isWalk", false);
            anim.SetBool("isRun", false);
        }
        else if (speed < run)
        {
            anim.SetBool("isWalk", true);
            anim.SetBool("isRun", false);
        }
        else
        {
            anim.SetBool("isWalk", true);
            anim.SetBool("isRun", true);
        }

        lookVector = pc.LookVector;
        lookVector.x = 0;
        transform.rotation = Quaternion.Euler(lookVector);

        previousPosition = pc.transform.position;
    }
    
    public void EjectLineRenderer(Vector3 hitPoint)
    {
        LeanPool.Despawn(LeanPool.Spawn(gunFirePrefab, shotPoint.position, shotPoint.rotation), 3f);
        
        
        var tr = LeanPool.Spawn(tracerPrefab);
        var lr = tr.GetComponent<LineRenderer>();
        var start = shotPoint.position;
        var end = hitPoint;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.widthMultiplier = 0.5f;
        DOTween.To(() => lr.widthMultiplier, v => lr.widthMultiplier = v, 0f, 0.07f).SetUpdate(true).OnComplete(() => LeanPool.Despawn(tr));
    }
}
