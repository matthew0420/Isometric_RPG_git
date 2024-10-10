using System.Collections.Generic;
using System;
using UnityEngine;

//generic enum needed upon instantiation 
public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    //Dict for storing concrete states, only accessible from classes that inherit from StateManager
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    public BaseState<EState> CurrentState { get; protected set; }

    //prevents update from calling state transition multiple times during processing
    protected bool IsTransitioningState = false;
    private void Start()
    {
        CurrentState.EnterState();
    }

    private void Update()
    {
        EState nextStateKey = CurrentState.GetNextState();
        
        if(!IsTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        }
        else if (!IsTransitioningState)
        {
            TransitionToState(nextStateKey);
        }
    }

    public void TransitionToState(EState statekey)
    {
        Debug.Log($"Transitioning from {CurrentState.StateKey} to {statekey}");
        
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[statekey];
        CurrentState.EnterState();
        IsTransitioningState = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CurrentState.OnTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        CurrentState.OnTriggerExit(other);
    }
}
