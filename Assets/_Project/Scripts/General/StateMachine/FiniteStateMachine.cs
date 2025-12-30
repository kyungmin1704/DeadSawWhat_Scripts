using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
    public BaseState defaultState;

    private BaseState currentState;
    public BaseState CurrentState
    {
        get => currentState;
        set
        {
            currentState?.Exit();
            currentState = value;
            currentState?.Enter();
        }
    }

    protected void Update()
    {
        CurrentState.UpdateLogic();
    }

    protected void FixedUpdate()
    {
        CurrentState.UpdatePhysics();
    }
}
