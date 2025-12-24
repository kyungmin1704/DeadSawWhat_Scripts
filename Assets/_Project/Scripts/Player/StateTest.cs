using DG.Tweening;
using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTest : MonoBehaviour
{
    Animator anim;
    Vector3 previousPosition;
    Vector3 shotPoint;

    public float walk = 0.1f;
    public float run = 1.0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    void Update()
    {
        Vector3 movement = transform.position - previousPosition;
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

        previousPosition = transform.position;
    }
}
