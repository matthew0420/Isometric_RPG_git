using UnityEngine;
using System;

//Will pull from an Enum list of actions for each state
public abstract class BaseState<EState> where EState : Enum
{
    //constructor
    public BaseState(EState key)
    {
        StateKey = key;
    }
    
    public EState StateKey { get; private set; }
    
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    //generic return with placeholder type
    public abstract EState GetNextState();
    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerStay(Collider other);
    public abstract void OnTriggerExit(Collider other);
}
