using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> : MonoBehaviour where T : struct, System.Enum
{
    private T state;
    public T State
    {
        get => state;
        set
        {
            if (state.Equals(value)) return;
            OnEndState(state);
            state = value;
            OnBeginState(state);
        }
    }

    protected virtual void Update()
    {
        OnState(state);
    }

    protected virtual void OnBeginState(T state)
    {

    }

    protected virtual void OnState(T state)
    {

    }

    protected virtual void OnEndState(T state)
    {
        
    }
}
