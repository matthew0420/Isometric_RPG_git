using UnityEngine;

public class RiseState : EnvironmentInteractionState
{
    public RiseState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate)
    {
        EnvironmentInteractionContext Context = context;
    }
    
    public override void EnterState(){}
    public override void ExitState(){}
    public override void UpdateState(){}
    //generic return with placeholder type
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        return StateKey;
    }
    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerStay(Collider other){}
    public override void OnTriggerExit(Collider other){}
}
