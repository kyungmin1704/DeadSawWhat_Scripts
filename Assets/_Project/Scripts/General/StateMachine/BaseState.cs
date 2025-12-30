using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : MonoBehaviour
{
    public abstract void Init(FiniteStateMachine fsm);
    public abstract void Enter();
    public abstract void UpdateLogic();
    public abstract void UpdatePhysics();
    public abstract void Exit();
}
